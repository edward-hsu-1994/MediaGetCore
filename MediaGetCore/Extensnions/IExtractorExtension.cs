using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGetCore.Extensnions {
    /// <summary>
    /// 針對<see cref="IExtractor"/>的擴充方法
    /// </summary>
    public static class IExtractorExtension {
        /// <summary>
        /// 取得指定<see cref="Uri"/>字串頁面媒體
        /// </summary>
        /// <param name="obj"><see cref="IExtractor"/>實例</param>
        /// <param name="url">目標網址</param>
        /// <returns>媒體剖析結果陣列</returns>
        public static MediaInfo[] GetMediaInfo(this IExtractor obj, string url) {
            return obj.GetMediaInfosAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 取得指定<see cref="Uri"/>頁面媒體
        /// </summary>
        /// <param name="obj"><see cref="IExtractor"/>實例</param>
        /// <param name="url">目標網址</param>
        /// <returns>媒體剖析結果陣列</returns>
        public static MediaInfo[] GetMediaInfo(this IExtractor obj, Uri url) {
            return obj.GetMediaInfosAsync(url).GetAwaiter().GetResult();
        }
    }
}
