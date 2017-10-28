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
    /// 針對Dailymotion的媒體剖析器
    /// </summary>
    [ExtractorUrlMatch(@"http(s)?:\/\/www.dailymotion.com\/video\/.+")]
    public class DailymotionExtractor : ExtractorBase {
        /// <inheritdoc />
        public override Task<MediaInfo[]> GetMediaInfosAsync(string url) {
            return _GetMediaInfosAsync(url);
        }

        /// <summary>
        /// 用以ReTry用，為主要影音擷取方法
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="time">重測剩餘次數</param>
        /// <returns>剖析結果</returns>
        private async Task<MediaInfo[]> _GetMediaInfosAsync(string url, int time = 1) {
            if (!IsMatch(url)) throw new UrlNotSupportedException();

            var pageHTML = await DownloadHelper.DownloadHtmlAsync(url);

            JObject mediaJObject;
            try {
                mediaJObject = GetMediaJObject(pageHTML);
            } catch { // 偶爾會完全取不到資料
                if (time <= 0) throw new InvalidOperationException("無法取得頁面HTML");
                return await _GetMediaInfosAsync(url, --time);
            }

            var template = new MediaInfo() {
                Name = mediaJObject["metadata"]["title"].Value<string>(),
                Description = pageHTML
                    .DocumentNode.SelectSingleNode("//meta[@property='og:description']")
                    ?.GetAttributeValue("content", null),
                Duration = mediaJObject["metadata"]["duration"].Value<double>(),
                SourceUrl = new Uri(url),
                Thumbnail = new Uri(mediaJObject["metadata"]["filmstrip_url"].Value<string>()),
                ExtractorType = GetType(),
                Type = MediaTypes.Video
            };

            var result = new List<MediaInfo>();

            var qualities = mediaJObject["metadata"]["qualities"].Value<JObject>();

            foreach (var quality in qualities.Properties().Select(x => x.Name)) {
                if (quality == "auto") continue;

                foreach (var format in qualities[quality].Value<JArray>()) {
                    var resultItem = (MediaInfo)template.Clone();
                    resultItem.RealUrl = new Uri(format["url"].Value<string>());
                    resultItem.Attributes["quality"] = quality;
                    resultItem.Attributes["mime"] = format["type"].Value<string>();
                    if (resultItem.Attributes["mime"].IndexOf("/mp4") > -1) { // 只有MP4有大小
                        resultItem.Attributes["size"] = GetSize(format["url"].Value<string>());
                    }
                    result.Add(resultItem);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// 取得影片Size
        /// </summary>
        /// <param name="realUrl">媒體來源</param>
        /// <returns>影片大小</returns>
        private string GetSize(string realUrl) {
            return realUrl.InnerString("-", "/");
        }

        /// <summary>
        /// 自取得的<see cref="HtmlDocument"/>中取出Dailymotion影片設定的<see cref="JObject"/>
        /// </summary>
        /// <param name="document">目標網址的Html</param>
        /// <returns>影片設定資訊</returns>
        private JObject GetMediaJObject(HtmlDocument document) {
            var allScripts = document.DocumentNode.Descendants("script");
            string playerConfig = (from t in allScripts
                                   where t.InnerHtml?.IndexOf("__PLAYER_CONFIG__") > -1
                                   select t.InnerHtml).First();
            playerConfig = playerConfig.Substring(0, playerConfig.IndexOf("var __PLAYER_BODY__"));
            var jsonString = JavascriptHelper.Run(
                    playerConfig + ";JSON.stringify(__PLAYER_CONFIG__);"
                );
            return JObject.Parse(jsonString);
        }
    }
}
