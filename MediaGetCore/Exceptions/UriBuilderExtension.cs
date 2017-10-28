using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaGetCore.Exceptions {
    /// <summary>
    /// 針對<see cref="UriBuilder"/>的擴充方法
    /// </summary>
    public static class UriBuilderExtension {
        /// <summary>
        /// 取得所有參數字典
        /// </summary>
        /// <param name="obj"><see cref="UriBuilder"/>實例</param>
        /// <returns>參數字典</returns>
        public static Dictionary<string, string> GetParameterDictionary(this UriBuilder obj) {
            return obj.Query?.Substring(1)
                .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(
                    item => item[0],
                    item => Uri.UnescapeDataString(item[1])
                );
        }

        /// <summary>
        /// 設定所有參數字典
        /// </summary>
        /// <param name="obj"><see cref="UriBuilder"/>實例</param>
        /// <param name="data">參數字典</param>
        public static void SetParameterDictionary(this UriBuilder obj, Dictionary<string, string> data) {
            if (data.Count == 0) return;
            obj.Query = string.Join("&", data.Select(item => $"{item.Key}={Uri.EscapeDataString(item.Value)}"));
        }

        /// <summary>
        /// 取得指定參數
        /// </summary>
        /// <param name="obj"><see cref="UriBuilder"/>實例</param>
        /// <param name="key">參數名稱</param>
        /// <returns>參數值</returns>
        public static string GetQueryParameter(this UriBuilder obj, string key) {
            var data = obj.GetParameterDictionary();
            if (data.ContainsKey(key)) return data[key];
            return null;
        }

        /// <summary>
        /// 刪除指定參數
        /// </summary>
        /// <param name="obj"><see cref="UriBuilder"/>實例</param>
        /// <param name="key">參數名稱</param>
        public static void RemoveQueryParameter(this UriBuilder obj, string key) {
            var data = obj.GetParameterDictionary();
            data.Remove(key);
            obj.SetParameterDictionary(data);
        }

        /// <summary>
        /// 設定參數值
        /// </summary>
        /// <param name="obj"><see cref="UriBuilder"/>實例</param>
        /// <param name="key">參數名稱</param>
        /// <param name="value">參數值</param>
        public static void SetQueryParameter(this UriBuilder obj, string key, string value) {
            var data = obj.GetParameterDictionary();
            data[key] = value;
            obj.SetParameterDictionary(data);
        }
    }
}
