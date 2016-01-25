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
    /// 針對Dailymotion的剖析器
    /// </summary>
    public class DailymotionExtractor : ExtractorBase {
        public override event CompletedEvent OnCompleted;
        public override event ProcessEvent OnProcess;

        public override async Task<MediaInfo[]> GetMediaInfosAsync(Uri Url) {
            if (!this.IsMatch(Url)) throw new NotSupportedException("不正確的網址");
            string EmbedUrl = $"http://www.dailymotion.com/embed/video/{GetMediaId(Url.OriginalString)}";//取得外連播放器連結
            JObject MediaJObject = await GetMediaJson(EmbedUrl);
            OnProcess?.Invoke(this, 0.2);

            var p = MediaJObject["metadata"]?["qualities"]?["240"]?[0].Value<JObject>();

            JObject Qualities = MediaJObject["metadata"]?["qualities"]?.Value<JObject>();

            MediaInfo Templet = new MediaInfo();//資料樣板
            Templet.ExtractorType = typeof(DailymotionExtractor);
            Templet.SourceUrl = Url;
            Templet.Name = MediaJObject["metadata"]?["title"].Value<string>();
            Templet.Attributes["author"] = MediaJObject["metadata"]?["owner"]?["username"].Value<string>();
            Templet.Duration = MediaJObject["metadata"]?["duration"].Value<int>() ?? 0;
            Templet.Thumbnail = new Uri(MediaJObject["metadata"]?["poster_url"].Value<string>());

            List<MediaInfo> result = new List<MediaInfo>();

            double Process = 0.2;

            foreach (var i in Qualities) {
                int temp = 0;
                if (!int.TryParse(i.Key, out temp)) { continue; }

                JObject VideoData = i.Value[0].Value<JObject>();
                MediaInfo t = Templet.Clone();
                t.Type = MediaTypes.Video;
                t.RealUrl = new Uri(VideoData?["url"].Value<string>());
                t.Attributes["quality"] = i.Key;
                t.Attributes["size"] = GetSize(VideoData?["url"].Value<string>());
                t.Attributes["mime"] = "video/mp4";

                result.Add(t);

                Process += 0.8 / Qualities.Count;
                OnProcess?.Invoke(this, Process);
            }

            MediaInfo[] output = result.ToArray();
            OnCompleted?.Invoke(this, output);
            return output;
        }

        /// <summary>
        /// 取得媒體ID
        /// </summary>
        /// <param name="Url">網址</param>
        /// <returns>媒體ID</returns>
        private string GetMediaId(string Url) {
            string result = new Uri(Url).Segments.Last<string>();
            result = result.Substring(0, result.IndexOf("_"));
            return result;
        }

        /// <summary>
        /// 取得媒體JObject
        /// </summary>
        /// <param name="Url">媒體來源</param>
        /// <returns>媒體資訊</returns>
        private async Task<JObject> GetMediaJson(string Url) {            
            #region 取得目標JS區段
            HtmlDocument HTMLDoc = await this.DownloadHtmlAsync(Url);
            var Scripts = HTMLDoc.DocumentNode.Descendants("script");
            string MediaJs = null;
            foreach (var Script in Scripts) {
                MediaJs = Script.InnerHtml;
                if (MediaJs != null && MediaJs.IndexOf("var info") > -1) break;
            }


            MediaJs = "var document={getElementById:function(){return null;}},parent,DM_DispatchEvent={dispatch:function(){}},window={playerV5:{}},location={host:{match:function(){return false;}},search:''},DailymotionPlayer={},console={},dmp={create:function(a,b){return b;}};" + MediaJs + ";console.log(JSON.stringify(window.playerV5));";
            #endregion
            return JObject.Parse(NodeJsFactory.RunScript(MediaJs));
        }

        /// <summary>
        /// 取得影片Size
        /// </summary>
        /// <param name="RealUrl">媒體來源</param>
        /// <returns>影片大小</returns>
        private string GetSize(string RealUrl) {
            return RealUrl.InnerString("-", "/");
        }
    }
}
