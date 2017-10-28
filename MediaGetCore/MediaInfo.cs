using System;
using System.Collections.Generic;
using MediaGetCore.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MediaGetCore {
    /// <summary>
    /// 媒體剖析器剖析結果
    /// </summary>
    public class MediaInfo : ICloneable {
        /// <summary>
        /// 名稱或標題
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 敘述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MediaTypes Type { get; set; }

        /// <summary>
        /// 來源網址
        /// </summary>
        public Uri SourceUrl { get; set; }

        /// <summary>
        /// 真實位址
        /// </summary>
        public Uri RealUrl { get; set; }

        /// <summary>
        /// 長度(秒)
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// 縮圖網址
        /// </summary>
        public Uri Thumbnail { get; set; }

        /// <summary>
        /// 其他相關屬性
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 剖析結果來源類型
        /// </summary>
        [JsonConverter(typeof(StringTypeConverter))]
        public Type ExtractorType { get; set; }

        /// <summary>
        /// 取得目前實例的深層副本
        /// </summary>
        /// <returns>目前實例的深層副本</returns>
        object ICloneable.Clone() {
            MediaInfo result = new MediaInfo();
            result.Name = this.Name;
            result.Type = this.Type;
            result.SourceUrl = this.SourceUrl;
            result.RealUrl = this.RealUrl;
            result.ExtractorType = this.ExtractorType;

            foreach (var keyvalue in this.Attributes)
                result.Attributes[keyvalue.Key] = keyvalue.Value;

            return result;
        }
    }
}
