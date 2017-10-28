using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaGetCore.Attributes {
    /// <summary>
    /// 標示剖析器檢驗合法輸入<see cref="Uri"/>的<see cref="Regex"/>字串
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ExtractorUrlMatchAttribute : Attribute {
        /// <summary>
        /// 取得用以檢驗的<see cref="Regex"/>實例
        /// </summary>
        public Regex UrlRegex { get; }

        /// <summary>
        /// 輸入指定正初始化剖析檢驗正規表示法字串
        /// </summary>
        /// <param name="urlRegex">正規表示法字串</param>
        public ExtractorUrlMatchAttribute(string urlRegex) {
            this.UrlRegex = new Regex(urlRegex);
        }

        /// <summary>
        /// 檢驗輸入<see cref="Uri"/>字串是否符合指定的規範
        /// </summary>
        /// <param name="url">欲檢驗的<see cref="Uri"/>字串</param>
        /// <returns>是否符合規範</returns>
        public bool IsMatch(string url) => this.UrlRegex.IsMatch(url);
    }
}
