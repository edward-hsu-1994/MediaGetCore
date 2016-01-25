using MediaGetCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MediaGetCore.Extractors{
    /// <summary>
    /// 針對Xuite的剖析器
    /// </summary>
    public class XuiteExtractor : ExtractorBase {
        public override event ProcessEvent OnProcess;
        public override event CompletedEvent OnCompleted;

        public override async Task<MediaInfo[]> GetMediaInfosAsync(Uri Url) {
            if (!this.IsMatch(Url)) throw new NotSupportedException("不正確的網址");
            string mediaId = this.GetMediaId(Url);

            Dictionary<string, string> apiData = await this.GetMediaApiData(mediaId);
            OnProcess?.Invoke(this, 0.1);
            
            List<MediaInfo> result = new List<MediaInfo>();

            MediaInfo baseInfo = new MediaInfo();
            baseInfo.SourceUrl = Url;
            baseInfo.Name = apiData["title"];
            baseInfo.Duration = (int)double.Parse(apiData["duration"]);
            string description;
            apiData.TryGetValue("description",out description);
            baseInfo.Description = description;
            baseInfo.Thumbnail = new Uri(apiData["thumb"]);
            baseInfo.ExtractorType = this.GetType();
            baseInfo.Type = (MediaTypes)Enum.Parse(typeof(MediaTypes), apiData["type"], true);
            baseInfo.Attributes["author"] = apiData["author_name"];
            OnProcess?.Invoke(this, 0.3);
            if (apiData.ContainsKey("src")) {//360
                MediaInfo def_src = baseInfo.Clone();
                def_src.RealUrl = new Uri(apiData["src"]);

                if (baseInfo.Type == MediaTypes.Video) {
                    def_src.Attributes["size"] = "480x360";
                    def_src.Attributes["quality"] = "default";
                    def_src.Attributes["mime"] = "video/mp4";
                } else {
                    def_src.Attributes["mime"] = "audio/mp3";
                }
                result.Add(def_src);
            }

            OnProcess?.Invoke(this, 0.5);
            if (apiData.ContainsKey("flv_src")) {//360 flv
                MediaInfo flv_src = baseInfo.Clone();
                flv_src.RealUrl = new Uri(apiData["flv_src"]);

                if (baseInfo.Type == MediaTypes.Video) {
                    flv_src.Attributes["size"] = "480x360";
                    flv_src.Attributes["quality"] = "default";
                    flv_src.Attributes["mime"] = "video/x-flv";
                } else {
                    flv_src.Attributes["mime"] = "audio/x-flv";
                }
                result.Add(flv_src);
            }

            OnProcess?.Invoke(this, 0.7);
            if (apiData.ContainsKey("hq_src")) {//720
                MediaInfo hq_src = baseInfo.Clone();
                hq_src.RealUrl = new Uri(apiData["hq_src"]);

                if (baseInfo.Type == MediaTypes.Video) {
                    hq_src.Attributes["size"] = "1280x720";
                    hq_src.Attributes["quality"] = "hq";
                }
                hq_src.Attributes["mime"] = "video/mp4";

                result.Add(hq_src);
            }

            OnProcess?.Invoke(this, 0.9);
            if (apiData.ContainsKey("hd1080_src")) {//1080
                MediaInfo hd1080_src = baseInfo.Clone();
                hd1080_src.RealUrl = new Uri(apiData["hd1080_src"]);

                if (baseInfo.Type == MediaTypes.Video) {
                    hd1080_src.Attributes["size"] = "1920x1080";
                    hd1080_src.Attributes["quality"] = "hd";
                }
                hd1080_src.Attributes["mime"] = "video/mp4";

                result.Add(hd1080_src);
            }


            MediaInfo[] output = result.ToArray();
            OnProcess?.Invoke(this, 1);
            OnCompleted?.Invoke(this, output);
            return output;
        }

        private string Base64ToString(string Base64) {
            return Encoding.UTF8.GetString(Convert.FromBase64String(Base64));
        }

        private string StringToBase64(string Data) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(Data));
        }

        private async Task<Dictionary<string,string>> GetMediaApiData(string MediaId) {
            XmlDocument apiData = new XmlDocument();
            apiData.LoadXml(await this.DownloadStringAsync($"http://vlog.xuite.net/flash/player?media={MediaId}"));

            XmlNodeList Properties = apiData.SelectNodes("//property");//取得屬性節點集合

            Dictionary<string, string> Data = new Dictionary<string, string>();
            foreach (XmlNode Property in Properties) {//迴圈讀取所有屬性節點資料
                string PropertyName = this.Base64ToString(Property.Attributes["id"].Value);
                //取得屬性的名稱

                string PropertyValue = Uri.UnescapeDataString(this.Base64ToString(Property.InnerText));
                //取得屬性的值

                if (string.IsNullOrWhiteSpace(PropertyValue))
                    continue;
                //如果為空字串則該屬性不用加入值組物件中

                Data[PropertyName] = PropertyValue;//加入值組物件
            }
            return Data;//回傳
        }

        private string GetMediaId(Uri Url) {
            return this.StringToBase64(this.Base64ToString(Url.Segments.Last().Replace("/","")).InnerString("-","."));
        }
    }
}
