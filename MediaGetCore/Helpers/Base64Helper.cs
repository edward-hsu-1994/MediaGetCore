using System;
using System.Collections.Generic;
using System.Text;

namespace MediaGetCore.Helpers {
    /// <summary>
    /// 針對BASE64編碼幫助類別
    /// </summary>
    public static class Base64Helper {
        /// <summary>
        /// 將BASE64內容轉換為字串
        /// </summary>
        /// <param name="base64">BASE64編碼內容</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>轉換字串結果</returns>
        public static string Base64ToString(string base64, Encoding encoding = null) {
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// 將字串轉換為BASE64內容
        /// </summary>
        /// <param name="str">字串</param>
        /// <param name="encoding">字串編碼，如為<see cref="null"/>則表示使用預設值<see cref="Encoding.UTF8"/></param>
        /// <returns>轉換BASE64結果</returns>
        public static string StringToBase64(string str, Encoding encoding = null) {
            encoding = encoding ?? Encoding.UTF8;
            return Convert.ToBase64String(encoding.GetBytes(str));
        }
    }
}
