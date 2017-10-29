using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EzCoreKit.Extensions;
using HtmlAgilityPack;
using MediaGetCore.Attributes;
using MediaGetCore.Exceptions;
using MediaGetCore.Helpers;
using Newtonsoft.Json.Linq;

namespace MediaGetCore.Extractors {
    /// <summary>
    /// 針對Facebook的媒體剖析器
    /// </summary>
    [ExtractorUrlMatch(@"^https?://www.facebook.com/.+/videos/\d+")]
    public sealed class FacebookExtractor : ExtractorBase {
        /// <inheritdoc />
        public override async Task<MediaInfo[]> GetMediaInfosAsync(string url) {
            if (!IsMatch(url)) throw new UrlNotSupportedException();

            var pageHTML = await DownloadHelper.DownloadHtmlAsync(url);
            var videoData = GetVideoData(pageHTML);

            if (videoData["is_live_stream"].Value<bool>()) {
                throw new LiveNotSupportedException();
            }

            var description = GetDescription(pageHTML);
            var duration = GetDuration(videoData);

            var template = new MediaInfo() {
                Name = pageHTML.DocumentNode.SelectSingleNode("//title").InnerText,
                Description = description,
                SourceUrl = new Uri(url),
                Duration = duration,
                ExtractorType = GetType(),
                Type = MediaTypes.Video,
                Thumbnail = GetThumbnail(pageHTML)
            };

            var sd_no_ratelimit = videoData["sd_src_no_ratelimit"].Value<string>();
            var sd = videoData["sd_src"].Value<string>();
            var hd_no_ratelimit = videoData["hd_src_no_ratelimit"].Value<string>();
            var hd = videoData["hd_src"].Value<string>();

            List<MediaInfo> result = new List<MediaInfo>();
            if (!string.IsNullOrEmpty(sd)) {
                var item = (MediaInfo)template.Clone();
                item.Attributes["quality"] = "sd";
                item.RealUrl = new Uri(sd);
                result.Add(item);
            }
            if (!string.IsNullOrEmpty(hd)) {
                var item = (MediaInfo)template.Clone();
                item.Attributes["quality"] = "hd";
                item.RealUrl = new Uri(hd);
                result.Add(item);
            }
            if (!string.IsNullOrEmpty(sd_no_ratelimit)) {
                var item = (MediaInfo)template.Clone();
                item.Attributes["quality"] = "sd_no_ratelimit";
                item.RealUrl = new Uri(sd_no_ratelimit);
                result.Add(item);
            }
            if (!string.IsNullOrEmpty(hd_no_ratelimit)) {
                var item = (MediaInfo)template.Clone();
                item.Attributes["quality"] = "hd_no_ratelimit";
                item.RealUrl = new Uri(hd_no_ratelimit);
                result.Add(item);
            }

            return result.ToArray();
        }

        /// <summary>
        /// 取得影片預覽圖
        /// </summary>
        /// <param name="pageHTML">來源HTML</param>
        /// <returns>預覽圖連結</returns>
        private Uri GetThumbnail(HtmlDocument pageHTML) {
            var code = pageHTML.DocumentNode.SelectNodes("//code").First(x => x.InnerHtml.Contains("<video"));
            var feedHTML = code.InnerHtml.Replace("<!--", string.Empty).Replace("-->", string.Empty);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(feedHTML);

            string style = HttpUtility.HtmlDecode(htmlDoc.DocumentNode.Descendants("img")
                .Where(d =>
                   d.Attributes.Contains("style")
                ).First().Attributes["style"].Value);
            style = style.Replace("\\\\", "%");
            style = Uri.UnescapeDataString(style);
            style = style.Replace(" ", string.Empty);
            style = style.InnerString("background-image:url('", "');");

            return new Uri(style);
        }

        /// <summary>
        /// 取得影片長度
        /// </summary>
        /// <param name="videoData">影片資訊</param>
        /// <returns>影片長度</returns>
        private double GetDuration(JObject videoData) {
            var timeString = videoData["dash_manifest"].Value<string>().InnerString("mediaPresentationDuration=\"", "\"");
            timeString = timeString.Replace("PT", string.Empty).Replace("S", string.Empty);
            timeString = timeString.Replace("H", ":").Replace("M", ":");

            return TimeSpan.Parse(timeString).TotalSeconds;
        }

        /// <summary>
        /// 取得影片敘述
        /// </summary>
        /// <param name="pageHTML">來源HTML</param>
        /// <returns>影片敘述</returns>
        private string GetDescription(HtmlDocument pageHTML) {
            var code = pageHTML.DocumentNode.SelectNodes("//code").First(x => x.InnerHtml.Contains("role=\"feed\""));
            var feedHTML = code.InnerHtml.Replace("<!--", string.Empty).Replace("-->", string.Empty);
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(feedHTML);

            return HttpUtility.HtmlDecode(htmlDoc.DocumentNode.Descendants("div")
                .Where(d =>
                   d.Attributes.Contains("class")
                   &&
                   d.Attributes["class"].Value.Contains("userContent")
                ).First().InnerText);
        }

        /// <summary>
        /// 自HTML中取得影片資訊
        /// </summary>
        /// <param name="pageHTML">HTML來源</param>
        /// <returns>影片資訊</returns>
        private JObject GetVideoData(HtmlDocument pageHTML) {
            var str = pageHTML.DocumentNode.InnerHtml.InnerString("videoData:", ",minQuality");
            str = str.Replace("\\x3C", "<");
            var ary = JArray.Parse(str);
            return ary.First as JObject;
        }
    }
}
