using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzCoreKit.Extensions;
using HtmlAgilityPack;
using MediaGetCore.Attributes;
using MediaGetCore.Exceptions;
using MediaGetCore.Helpers;
using Newtonsoft.Json.Linq;

namespace MediaGetCore.Extractors {
    /// <summary>
    /// 針對Youtube的媒體剖析器
    /// </summary>
    [ExtractorUrlMatch(@"http(s)?://www.youtube.com/watch\?v=.+")]
    public sealed class YoutubeExtractor : ExtractorBase {
        /// <summary>
        /// 用以映射MIME類型與<see cref="MediaTypes"/>
        /// </summary>
        private static Dictionary<string, MediaTypes> mimeMap = new Dictionary<string, MediaTypes>() {
            ["audio"] = MediaTypes.Audio,
            ["video"] = MediaTypes.Video
        };

        /// <inheritdoc />
        public override async Task<MediaInfo[]> GetMediaInfosAsync(string url) {
            // 檢查網址是否符合規範
            if (!IsMatch(url)) throw new UrlNotSupportedException();

            var pageHTML = await DownloadHelper.DownloadHtmlAsync(url);
            var mediaJObject = GetMediaJObject(pageHTML);

            // 檢驗是否為直播串流
            if (mediaJObject["args"]["livestream"]?.Value<string>() == "1") {
                throw new LiveNotSupportedException(); // 不支援
            }

            // 結果樣板
            var template = new MediaInfo() {
                SourceUrl = new Uri(url),
                ExtractorType = GetType(),
                Name = mediaJObject["args"]["title"].Value<string>(),
                Duration = mediaJObject["args"]["length_seconds"].Value<int>(),
                Thumbnail = new Uri(mediaJObject["args"]["thumbnail_url"].Value<string>()),
                Description = pageHTML
                    .DocumentNode.SelectSingleNode("//meta[@name='description']")
                    .GetAttributeValue("content", null)
            };

            // 取得解密函數
            var decodingFunction =
                await GetDecodingFunc("https://www.youtube.com" + mediaJObject["assets"]["js"].Value<string>());

            var streamFormatMap = GetStreamFormatMap(mediaJObject);
            var streamMap = GetStreamMap(mediaJObject);

            var result = new List<MediaInfo>();
            Parallel.ForEach(streamMap, item => {
                var resultItem = (MediaInfo)template.Clone();
                resultItem.Type = mimeMap[
                    new string(
                        item["type"]["mime"].Value<string>()
                        .TakeWhile(Ch => Ch != '/').ToArray()
                    )];

                #region 連結解密
                UriBuilder realUrlBuilder = new UriBuilder(item["url"].Value<string>());
                var urlSignature = realUrlBuilder.GetQueryParameter("s") ??
                                  realUrlBuilder.GetQueryParameter("sig") ??
                                  realUrlBuilder.GetQueryParameter("signature");
                var itemSignature = item["s"]?.Value<string>() ??
                                    item["sig"]?.Value<string>() ??
                                    item["signature"]?.Value<string>();
                realUrlBuilder.SetQueryParameter(
                    "signature",
                    decodingFunction(
                        urlSignature ?? itemSignature, urlSignature != null));

                resultItem.RealUrl = realUrlBuilder.Uri;
                #endregion

                #region 擴充屬性
                resultItem.Attributes["mime"] = item["type"]["mime"].Value<string>();
                resultItem.Attributes["codecs"] = item["type"]["codecs"]?.Value<string>();
                resultItem.Attributes["author"] = mediaJObject["args"]["author"].Value<string>();
                if (resultItem.Type == MediaTypes.Video) {
                    resultItem.Attributes["size"] = item["size"]?.Value<string>() ?? streamFormatMap[item["itag"]?.Value<string>()];
                    resultItem.Attributes["quality"] = item["quality"]?.Value<string>();
                } else if (resultItem.Type == MediaTypes.Audio) {
                    resultItem.Attributes["bitrate"] = item["bitrate"].Value<string>();
                }
                #endregion

                result.Add(resultItem);
            });

            return result.ToArray();
        }

        /// <summary>
        /// 自取得的<see cref="HtmlDocument"/>中取出Youtube影片設定的<see cref="JObject"/>
        /// </summary>
        /// <param name="document">目標網址的Html</param>
        /// <returns>影片設定資訊</returns>
        private JObject GetMediaJObject(HtmlDocument document) {
            var allScripts = document.DocumentNode.Descendants("script");
            string playerConfig = (from t in allScripts
                                   where t.InnerHtml?.IndexOf("var ytplayer") > -1
                                   select t.InnerHtml).First();
            var jsonString = JavascriptHelper.Run(
                "var window={};" + playerConfig + ";JSON.stringify(ytplayer.config);"
                );
            return JObject.Parse(jsonString);
        }

        /// <summary>
        /// 自指定Youtube Javascript腳本中提取解密函數
        /// </summary>
        /// <param name="url">目標Javascript腳本網址</param>
        /// <returns>解密函數</returns>
        private async Task<Func<string, bool, string>> GetDecodingFunc(string url) {
            string playerScript = await DownloadHelper.DownloadStringAsync(url);
            string functionName = string.Empty;
            try {
                functionName = playerScript
                    .InnerString("\"signature\",", "))")
                    .InnerString("\"signature\",", "(");
            } catch {
                functionName = playerScript.InnerString("\"signature\",", "))");
                functionName = functionName.Substring(0, functionName.IndexOf("("));
            }

            if (functionName?.Length == 0) {// 找不到解密函數
                return new Func<string, bool, string>((value, inUrl) => value);
            }

            var functionBody = $"function{playerScript.InnerString($"\n{functionName}=function", "}")};" + "}";

            var functionRefName = functionBody.InnerString(";", ".");
            var functionRef = playerScript.InnerString("var " + functionRefName + "=", "};") + "}";

            var args = functionBody.InnerString("(", ")");

            functionBody = functionBody.Substring(functionBody.IndexOf("{") + 1);
            functionBody = "function(" + args + "){var " + functionRefName + "=" + functionRef + ";" + functionBody;

            return new Func<string, bool, string>((value, inUrl) => {
                var result = value;
                var scriptResult = JavascriptHelper.Run($"({functionBody})('{value}')");
                if (inUrl) {
                    // 如果signature來自於URL，如果輸入值長度非81且解密結果為81才是正確的，否則直接用URL的
                    if (value.Length != 81 && scriptResult.Length == 81) result = scriptResult;
                } else {
                    // 來自外部的signature都是需要解密的
                    result = scriptResult;
                }
                return result;
            });
        }

        /// <summary>
        /// 自影片設定資訊中取得影片格式清單
        /// </summary>
        /// <param name="mediaJObject">影片設定資訊</param>
        /// <returns>影片格式清單</returns>
        private Dictionary<string, string> GetStreamFormatMap(JObject mediaJObject) {
            string fmt_list = mediaJObject["args"]["fmt_list"].Value<string>();

            return fmt_list.Split(',')
                    .Select(item => item.Split('/'))
                    .ToDictionary(item => item[0], item => item[1]);
        }

        /// <summary>
        /// 自影片設定資訊中取得所有串流類型資訊
        /// </summary>
        /// <param name="mediaJObject">影片設定資訊</param>
        /// <returns>所有串流類型資訊</returns>
        private JObject[] GetStreamMap(JObject mediaJObject) {
            JObject[] GetStreamMapByKey(JObject _mediaJObject, string Key) {
                return _mediaJObject["args"][Key]
                    .Value<string>().Split(',') // 切割出各種不同的類型串流
                    .Select(item => item.Split('&')) // 把各種類型的串流的參數在切割出來
                    .Select(item => { // 處理KeyValue段落
                        return item.Select(item2 => {
                            string[] KeyValue = item2.Split(new char[] { '=' }, 2);
                            return new KeyValuePair<string, string>(KeyValue[0], KeyValue[1]);
                        }); // 本階段回傳KeyValuePair[][]
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
                        typeJObject["mime"] = type.Substring(0, hasCodecs == -1 ? type.Length : hasCodecs);
                        if (hasCodecs > -1) typeJObject["codecs"] = type.InnerString("\"", "\"")?.Replace("+", string.Empty);
                        item["type"] = typeJObject;
                        return item;
                    }).ToArray();
            }
            return GetStreamMapByKey(mediaJObject, "url_encoded_fmt_stream_map")
                .Union(GetStreamMapByKey(mediaJObject, "adaptive_fmts")).ToArray();
        }
    }
}
