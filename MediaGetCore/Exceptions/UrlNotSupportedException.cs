using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGetCore.Exceptions {
    /// <summary>
    /// 媒體剖析器不支援輸入網址例外
    /// </summary>
    public class UrlNotSupportedException : NotSupportedException {
        /// <summary>
        /// 初始化媒體剖析器不支援輸入網址例外實例
        /// </summary>
        public UrlNotSupportedException() : base("目前的媒體剖析器不支援您輸入的URL") { }
    }
}
