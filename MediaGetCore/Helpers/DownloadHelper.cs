using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace MediaGetCore.Helpers {
    /// <summary>
    /// 下載相關功能幫助類別
    /// </summary>
    internal static class DownloadHelper {
        /// <summary>
        /// 下載指定<see cref="Uri"/>字串目標字串內容
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>下載字串內容</returns>
        public static async Task<string> DownloadStringAsync(string url, Encoding encoding = null) {
            encoding = encoding ?? Encoding.UTF8;

            HttpClient client = new HttpClient();
            return await client.GetStringAsync(url);
        }

        /// <summary>
        /// 下載指定<see cref="Uri"/>字串目標<see cref="JToken"/>內容
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>下載<see cref="JToken"/>內容</returns>
        public static async Task<JToken> DownloadJTokenAsync(string url, Encoding encoding = null) {
            return JToken.Parse(await DownloadStringAsync(url, encoding));
        }

        /// <summary>
        /// 下載指定<see cref="Uri"/>字串目標<see cref="HtmlDocument"/>內容
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>下載<see cref="HtmlDocument"/>內容</returns>
        public static async Task<HtmlDocument> DownloadHtmlAsync(string url, Encoding encoding = null) {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(await DownloadStringAsync(url, encoding));
            return htmlDoc;
        }

        #region 多載

        /// <summary>
        /// 下載指定<see cref="Uri"/>目標字串內容
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>下載字串內容</returns>
        public static async Task<string> DownloadStringAsync(Uri url, Encoding encoding = null) => await DownloadStringAsync(url.OriginalString, encoding);

        /// <summary>
        /// 下載指定<see cref="Uri"/>目標<see cref="JToken"/>內容
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>下載<see cref="JToken"/>內容</returns>
        public static async Task<JToken> DownloadJTokenAsync(Uri url, Encoding encoding = null) {
            return await DownloadJTokenAsync(url.OriginalString, encoding);
        }

        /// <summary>
        /// 下載指定<see cref="Uri"/>目標<see cref="HtmlDocument"/>內容
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>下載<see cref="HtmlDocument"/>內容</returns>
        public static async Task<HtmlDocument> DownloadHtmlAsync(Uri url, Encoding encoding = null) {
            return await DownloadHtmlAsync(url.OriginalString, encoding);
        }

        #endregion
    }
}
