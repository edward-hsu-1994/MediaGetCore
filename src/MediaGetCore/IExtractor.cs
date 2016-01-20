using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGetCore{
    /// <summary>
    /// 媒體剖析器通用接口
    /// </summary>
    public interface IExtractor {
        /// <summary>
        /// 取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結字串</param>
        /// <returns>影片資訊集合</returns>
        MediaInfo[] GetMediaInfos(string Url);

        /// <summary>
        /// 取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結Uri</param>
        /// <returns>影片資訊集合</returns>
        MediaInfo[] GetMediaInfos(Uri Url);

        /// <summary>
        /// 非同步取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結字串</param>
        /// <returns>影片資訊集合</returns>
        Task<MediaInfo[]> GetMediaInfosAsync(string Url);

        /// <summary>
        /// 非同步取得指定連結的影片資訊
        /// </summary>
        /// <param name="Url">指定連結Uri</param>
        /// <returns>影片資訊集合</returns>
        Task<MediaInfo[]> GetMediaInfosAsync(Uri Url);
        
        /// <summary>
        /// 檢查指定連結是否符合該Extractor規範
        /// </summary>
        /// <param name="Url">指定連結字串</param>
        /// <returns>檢查結果</returns>
        bool IsMatch(string Url);

        /// <summary>
        /// 檢查指定連結是否符合該Extractor規範
        /// </summary>
        /// <param name="Url">指定連結Uri</param>
        /// <returns>檢查結果</returns>
        bool IsMatch(Uri Url);

        /// <summary>
        /// 解析進度事件
        /// </summary>
        event ProcessEvent OnProcess;

        /// <summary>
        /// 剖析完成事件
        /// </summary>
        event CompletedEvent OnCompleted;
    }
}
