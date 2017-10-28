using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using EzCoreKit.Extensions;
using MediaGetCore.Attributes;
using MediaGetCore.Exceptions;
using MediaGetCore.Helpers;

namespace MediaGetCore.Extractors {
    /// <summary>
    /// 針對Xuite的媒體剖析器
    /// </summary>
    [ExtractorUrlMatch(@"(^https?://vlog.xuite.net/play/\S+)|(^https?://m.xuite.net/vlog/\S+/\S+)")]
    public class XuiteExtractor : ExtractorBase {
        /// <inheritdoc />
        public override async Task<MediaInfo[]> GetMediaInfosAsync(string url) {
            if (!IsMatch(url)) throw new UrlNotSupportedException();
            var tempUri = new Uri(url);
            var mediaId = GetMediaId(tempUri);

            var apiData = await GetMediaApiData(mediaId);

            var result = new List<MediaInfo>();

            var template = new MediaInfo();
            template.SourceUrl = tempUri;
            template.Name = apiData["title"];
            template.Duration = (int)double.Parse(apiData["duration"]);

            string description;
            apiData.TryGetValue("description", out description);
            template.Description = description;
            template.Thumbnail = new Uri(apiData["thumb"]);
            template.ExtractorType = GetType();
            template.Type = (MediaTypes)Enum.Parse(typeof(MediaTypes), apiData["type"], true);
            template.Attributes["author"] = apiData["author_name"];

            if (apiData.ContainsKey("src")) { // 360
                var def_src = (MediaInfo)template.Clone();
                def_src.RealUrl = new Uri(tempUri.Scheme + ":" + apiData["src"]);

                if (template.Type == MediaTypes.Video) {
                    def_src.Attributes["size"] = "480x360";
                    def_src.Attributes["quality"] = "default";
                    def_src.Attributes["mime"] = "video/mp4";
                } else {
                    def_src.Attributes["mime"] = "audio/mp3";
                }
                result.Add(def_src);
            }

            if (apiData.ContainsKey("flv_src")) { // 360 flv
                var flv_src = (MediaInfo)template.Clone();
                flv_src.RealUrl = new Uri(tempUri.Scheme + ":" + apiData["flv_src"]);

                if (template.Type == MediaTypes.Video) {
                    flv_src.Attributes["size"] = "480x360";
                    flv_src.Attributes["quality"] = "default";
                    flv_src.Attributes["mime"] = "video/x-flv";
                } else {
                    flv_src.Attributes["mime"] = "audio/x-flv";
                }
                result.Add(flv_src);
            }

            if (apiData.ContainsKey("hq_src")) { // 720
                var hq_src = (MediaInfo)template.Clone();
                hq_src.RealUrl = new Uri(tempUri.Scheme + ":" + apiData["hq_src"]);

                if (template.Type == MediaTypes.Video) {
                    hq_src.Attributes["size"] = "1280x720";
                    hq_src.Attributes["quality"] = "hq";
                }
                hq_src.Attributes["mime"] = "video/mp4";

                result.Add(hq_src);
            }

            if (apiData.ContainsKey("hd1080_src")) { // 1080
                var hd1080_src = (MediaInfo)template.Clone();
                hd1080_src.RealUrl = new Uri(tempUri.Scheme + ":" + apiData["hd1080_src"]);

                if (template.Type == MediaTypes.Video) {
                    hd1080_src.Attributes["size"] = "1920x1080";
                    hd1080_src.Attributes["quality"] = "hd";
                }
                hd1080_src.Attributes["mime"] = "video/mp4";

                result.Add(hd1080_src);
            }

            return result.ToArray();
        }

        /// <summary>
        /// 取得指定網址媒體唯一識別號
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <returns>唯一識別號</returns>
        private string GetMediaId(Uri url) {
            return Base64Helper.StringToBase64(
                Base64Helper.Base64ToString(url.Segments.Last().Replace("/", string.Empty)).InnerString("-", ".")
            );
        }

        /// <summary>
        /// 取得媒體資訊
        /// </summary>
        /// <param name="mediaId">媒體唯一識別號</param>
        /// <returns>媒體資訊</returns>
        private async Task<Dictionary<string, string>> GetMediaApiData(string mediaId) {
            XmlDocument apiData = new XmlDocument();
            apiData.LoadXml(
                await DownloadHelper.DownloadStringAsync($"http://vlog.xuite.net/flash/player?media={mediaId}")
            );

            XmlNodeList properties = apiData.SelectNodes("//property"); // 取得屬性節點集合

            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (XmlNode property in properties) { // 迴圈讀取所有屬性節點資料
                // 取得屬性的名稱
                string propertyName = Base64Helper.Base64ToString(property.Attributes["id"].Value);

                // 取得屬性的值
                string propertyValue = Uri.UnescapeDataString(Base64Helper.Base64ToString(property.InnerText));

                // 如果為空字串則該屬性不用加入值組物件中
                if (string.IsNullOrWhiteSpace(propertyValue)) continue;

                data[propertyName] = propertyValue; // 加入值組物件
            }
            return data; // 回傳
        }
    }
}
