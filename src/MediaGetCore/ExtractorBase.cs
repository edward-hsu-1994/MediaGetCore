using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaGetCore{
    /// <summary>
    /// 已簡化輸入連結檢查方式與簡易非同步化方法的類別
    /// </summary>
    public abstract class ExtractorBase : IExtractor {
        /// <summary>
        /// 剖析過程進度事件
        /// </summary>
        public abstract event ProcessEvent OnProcess;

        /// <summary>
        /// 剖析完成事件
        /// </summary>
        public abstract event CompletedEvent OnCompleted;

        /// <summary>
        /// 取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結<see cref="string"/></param>
        /// <returns>影片資訊集合</returns>
        public MediaInfo[] GetMediaInfos(string Url) {
            return this.GetMediaInfosAsync(Url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結<see cref="Uri"/></param>
        /// <returns>影片資訊集合</returns>
        public MediaInfo[] GetMediaInfos(Uri Url) {
            return GetMediaInfos(Url.OriginalString);
        }

        /// <summary>
        /// 檢查指定連結是否符合該Extractor規範
        /// </summary>
        /// <param name="Url">指定連結<see cref="string"/></param>
        /// <returns>檢查結果</returns>
        public virtual bool IsMatch(string Url) {
            Regex regex = new Regex(Resource.MatchRegex[this.GetType()]);
            return regex.IsMatch(Url);
        }

        /// <summary>
        /// 檢查指定連結是否符合該Extractor規範
        /// </summary>
        /// <param name="Url">指定連結<see cref="Uri"/></param>
        /// <returns>檢查結果</returns>
        public bool IsMatch(Uri Url) {
            return IsMatch(Url.OriginalString);
        }

        /// <summary>
        /// 非同步取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結<see cref="string"/></param>
        /// <returns>影片資訊集合</returns>
        public async Task<MediaInfo[]> GetMediaInfosAsync(string Url) {
            return await GetMediaInfosAsync(new Uri(Url));
        }

        /// <summary>
        /// 非同步取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結<see cref="Uri"/></param>
        /// <returns>影片資訊集合</returns>
        public abstract Task<MediaInfo[]> GetMediaInfosAsync(Uri Url);


        #region 網路內容存取
        /// <summary>
        /// 非同步下載指定連結字串
        /// </summary>
        /// <param name="Url">指定連結<see cref="string"/></param>
        /// <returns>結果字串</returns>
        protected async Task<string> DownloadStringAsync(string Url) {
            HttpClient client = new HttpClient();
            return await client.GetStringAsync(Url);
        }

        /// <summary>
        /// 非同步下載指定連結字串
        /// </summary>
        /// <param name="Url">指定連結<see cref="Uri"/></param>
        /// <returns>結果字串</returns>
        protected async Task<string> DownloadStringAsync(Uri Url) {
            return await DownloadStringAsync(Url.OriginalString);
        }

        /// <summary>
        /// 非同步下載指定連結<see cref="JToken"/>
        /// </summary>
        /// <param name="Url">指定連結<see cref="string"/></param>
        /// <returns>結果JToken</returns>
        protected async Task<JToken> DownloadJToken(string Url) {
            return JToken.Parse(await DownloadStringAsync(Url));
        }

        /// <summary>
        /// 非同步下載指定連結<see cref="JToken"/>
        /// </summary>
        /// <param name="Url">指定連結<see cref="Uri"/></param>
        /// <returns>結果JToken</returns>
        protected async Task<JToken> DownloadJToken(Uri Url) {
            return await DownloadJToken(Url.OriginalString);
        }

        /// <summary>
        /// 非同步下載指定連結<see cref="HtmlDocument"/>
        /// </summary>
        /// <param name="Url">指定連結<see cref="string"/></param>
        /// <returns>結果HtmlDocument</returns>
        protected async Task<HtmlDocument> DownloadHtmlAsync(string Url) {
            HtmlDocument HTMLDoc = new HtmlDocument();
            HTMLDoc.LoadHtml(await DownloadStringAsync(Url));
            return HTMLDoc;
        }

        /// <summary>
        /// 非同步下載指定連結<see cref="HtmlDocument"/>
        /// </summary>
        /// <param name="Url">指定連結<see cref="Uri"/></param>
        /// <returns>結果HtmlDocument</returns>
        protected async Task<HtmlDocument> DownloadHtmlAsync(Uri Url) {
            return await DownloadHtmlAsync(Url.OriginalString);
        }
        #endregion
    }
}
