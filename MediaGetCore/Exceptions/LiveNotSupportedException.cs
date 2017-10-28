using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGetCore.Exceptions {
    /// <summary>
    /// 媒體剖析器不支援直播串流解析
    /// </summary>
    public class LiveNotSupportedException : NotSupportedException {
        /// <summary>
        /// 初始化媒體剖析器不支援輸入網址例外實例
        /// </summary>
        public LiveNotSupportedException() : base("目前的媒體剖析器不支援直播串流解析") { }
    }
}
