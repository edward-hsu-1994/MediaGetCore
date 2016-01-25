using MediaGetCore.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGetCore{
    /// <summary>
    /// 靜態資源類型
    /// </summary>
    public static class Resource{
        /// <summary>
        /// 剖析器輸入驗證正規表示式對應
        /// </summary>
        public static Dictionary<Type, string> MatchRegex { get; set; } = new Dictionary<Type, string>() {
            [typeof(YoutubeExtractor)] = @"http(s)?://www.youtube.com/watch\?v=.+",
            [typeof(XuiteExtractor)] = @"(^https?://vlog.xuite.net/play/\S+)|(^https?://m.xuite.net/vlog/\S+/\S+)",
            [typeof(DailymotionExtractor)] = @"http(s)?://www.dailymotion.com/video/\S+"
        };
    }
}