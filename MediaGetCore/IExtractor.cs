using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MediaGetCore {
    /// <summary>
    /// 媒體剖析器通用介面
    /// </summary>
    public interface IExtractor {
        /// <summary>
        /// 異步取得指定<see cref="Uri"/>字串頁面媒體
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <returns>媒體剖析結果陣列</returns>
        Task<MediaInfo[]> GetMediaInfoAsync(string url);

        /// <summary>
        /// 異步取得指定<see cref="Uri"/>頁面媒體
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <returns>媒體剖析結果陣列</returns>
        Task<MediaInfo[]> GetMediaInfoAsync(Uri url);

        /// <summary>
        /// 檢驗指定<see cref="Uri"/>字串是否符合目前<see cref="IExtractor"/>實例規範
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <returns>是否符合規範</returns>
        bool IsMatch(string url);

        /// <summary>
        /// 檢驗指定<see cref="Uri"/>是否符合目前<see cref="IExtractor"/>實例規範
        /// </summary>
        /// <param name="url">目標網址</param>
        /// <returns>是否符合規範</returns>
        bool IsMatch(Uri url);
    }
}
