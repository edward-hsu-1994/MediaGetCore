using HtmlAgilityPack;
using MediaGetCore.Extensions;
using MediaGetCore.Factories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGetCore.Extractors{
    /// <summary>
    /// 針對Youtube剖析器
    /// </summary>
    public class YoutubeExtractor : ExtractorBase {
        public override event ProcessEvent OnProcess;
        public override event CompletedEvent OnCompleted;
        public override async Task<MediaInfo[]> GetMediaInfosAsync(Uri Url) {
            if (!this.IsMatch(Url)) throw new NotSupportedException("不正確的網址");

            HtmlDocument youtubePage = await this.DownloadHtmlAsync(Url);
            
            OnProcess?.Invoke(this,0.2);

            JObject mediaJObject = this.GetMediaJObject(youtubePage);

            if(mediaJObject["args"]["livestream"]?.Value<string>() == "1") {
                throw new NotSupportedException("不支援直播串流解析");
            }

            string description = youtubePage.DocumentNode.SelectSingleNode("//meta[@name='description']").GetAttributeValue("content", null);

            NodeJsFactory.Func decoding = await this.GetDecodingSignature("https:" + (string)mediaJObject["assets"]["js"]);
            Dictionary<string, string> streamFormatList = this.GetStreamFormatList(mediaJObject);
            JObject[] streamMap = this.GetStreamMap(mediaJObject);

            Dictionary<string, MediaTypes> MIME = new Dictionary<string, MediaTypes>() {
                ["audio"] = MediaTypes.Audio,
                ["video"] = MediaTypes.Video
            };

            OnProcess?.Invoke(this, 0.5);
            double processValue = 0.5;


            List<MediaInfo> result = new List<MediaInfo>();
            foreach(var item in streamMap) {
                MediaInfo resultItem = new MediaInfo();
                #region 通用屬性
                resultItem.SourceUrl = Url;
                resultItem.ExtractorType = this.GetType();
                resultItem.Name = mediaJObject["args"]["title"].Value<string>();
                resultItem.Duration = mediaJObject["args"]["length_seconds"].Value<int>();
                resultItem.Description = description;
                resultItem.Type = MIME[
                    new string(
                        item["type"]["mime"].Value<string>()
                        .TakeWhile(Ch => Ch != '/').ToArray()
                    )
                ];
                
                #region 連結解密
                UriBuilder realUrlBuilder = new UriBuilder(item["url"].Value<string>());
                realUrlBuilder.SetQueryParam("signature", decoding(
                    realUrlBuilder.GetQueryParam("s") ??
                    realUrlBuilder.GetQueryParam("sig") ??
                    realUrlBuilder.GetQueryParam("signature") ??
                    item["s"]?.Value<string>() ??
                    item["sig"]?.Value<string>() ??
                    item["signature"]?.Value<string>()
                ));
                resultItem.RealUrl = realUrlBuilder.Uri;
                #endregion
                #endregion

                #region 擴充屬性
                resultItem.Attributes["mime"] = item["type"]["mime"].Value<string>();
                resultItem.Attributes["codecs"] = item["type"]["codecs"]?.Value<string>();
                
                if (resultItem.Type == MediaTypes.Video) {
                    resultItem.Attributes["size"] = item["size"]?.Value<string>() ?? streamFormatList[item["itag"]?.Value<string>()];
                    resultItem.Attributes["quality"] = item["quality"]?.Value<string>();
                } else if(resultItem.Type == MediaTypes.Audio) {
                    resultItem.Attributes["bitrate"] = item["bitrate"].Value<string>();
                }
                #endregion

                result.Add(resultItem);

                processValue = processValue + 0.5 / streamMap.Count();
                OnProcess?.Invoke(this, processValue);
            }

            MediaInfo[] output = result.ToArray();
            OnCompleted?.Invoke(this, output);
            return output;
        }

        /// <summary>
        /// 自取得的<see cref="HtmlDocument"/>中取出Youtube影片設定的<see cref="JObject"/>
        /// </summary>
        /// <param name="Document">目標網址的Html</param>
        /// <returns>影片設定資訊</returns>
        private JObject GetMediaJObject(HtmlDocument Document) {
            var allScripts = Document.DocumentNode.Descendants("script");
            string playerConfig = (from t in allScripts
                                   where t.InnerHtml?.IndexOf("var ytplayer") > -1
                                   select t.InnerHtml).First();
            return JObject.Parse(NodeJsFactory.RunScript(
                "var window={};" + playerConfig + "console.log(JSON.stringify(ytplayer.config));")
            );
        }

        /// <summary>
        /// 自指定的YoutubePlayer之類別庫中取得影片簽章解密函數
        /// </summary>
        /// <param name="Url">YoutubePlayer類別庫網址</param>
        /// <returns>解密函數委派</returns>
        private async Task<NodeJsFactory.Func> GetDecodingSignature(string Url) {
            string playerScript = await this.DownloadStringAsync(Url);
            string functionName = playerScript.InnerString("\"signature\",", "(");
            if(functionName == null) {
                return (Args) => (string)Args[0];
            } else {
                string functionBody = $"function" +
                    playerScript.InnerString($",{functionName}=function", "}") +
                    ";}";

                string functionRefName = functionBody.InnerString(";\n", ".");
                string functionRef = playerScript.InnerString($"var {functionRefName}=", ";var ");


                string args = functionBody.InnerString("(", ")");
                functionBody = functionBody.Substring(functionBody.IndexOf("{") + 1);

                functionBody = $"function({args})" + "{" + $"var {functionRefName}={functionRef};{functionBody}";

                return (Args) => {
                    string firstArgs = (string)Args[0];
                    if (firstArgs.Length == 81) return firstArgs;
                    return NodeJsFactory.EvalFunc(functionBody)(Args);
                };
            }
        }

        /// <summary>
        /// 自影片設定資訊中取得影片格式清單
        /// </summary>
        /// <param name="MediaJObject">影片設定資訊</param>
        /// <returns>影片格式清單</returns>
        private Dictionary<string,string> GetStreamFormatList(JObject MediaJObject) {
            string fmt_list = MediaJObject["args"]["fmt_list"].Value<string>();

            return fmt_list.Split(',')
                    .Select(item=>item.Split('/'))
                    .ToDictionary(item=>item[0],item=>item[1]);
        }
        
        private JObject[] GetStreamMapByKey(JObject MediaJObject,string Key) {
            return MediaJObject["args"][Key]
                .Value<string>().Split(',')//切割出各種不同的類型串流
                .Select(item => item.Split('&'))//把各種類型的串流的參數在切割出來
                .Select(item => {//處理KeyValue段落
                    return item.Select(item2 => {
                        string[] KeyValue = item2.Split(new char[] { '=' }, 2);
                        return new KeyValuePair<string, string>(KeyValue[0], KeyValue[1]);
                    });//本階段回傳KeyValuePair[][]
                }).Select(item => {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    foreach (var item2 in item) {
                        result.Add(item2.Key, Uri.UnescapeDataString(item2.Value));
                    }
                    return JObject.FromObject(result);
                }).Select(item => {
                    string type = item["type"].Value<string>();
                    int hasCodecs = type.IndexOf(";");
                    JObject typeJObject = new JObject();
                    typeJObject["mime"] = type.Substring(0, hasCodecs == -1?type.Length:hasCodecs);
                    if(hasCodecs > -1)typeJObject["codecs"] = type.InnerString("\"", "\"")?.Replace("+","");
                    item["type"] = typeJObject;
                    return item;
                }).ToArray();
        }

        private JObject[] GetStreamMap(JObject MediaJObject) {
            return this.GetStreamMapByKey(MediaJObject, "url_encoded_fmt_stream_map")
                .Union(this.GetStreamMapByKey(MediaJObject, "adaptive_fmts")).ToArray();
        }
    }
}