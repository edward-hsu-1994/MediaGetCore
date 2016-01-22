using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using MediaGetCore.JsonConverters;
using Newtonsoft.Json.Converters;

namespace MediaGetCore{
    /// <summary>
    /// 剖析結果的相關資訊
    /// </summary>
    public class MediaInfo{
        /// <summary>
        /// 媒體的名稱或標題
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 媒體類型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MediaTypes Type { get; set; }

        /// <summary>
        /// 媒體來源網址
        /// </summary>
        public Uri SourceUrl { get; set; }

        /// <summary>
        /// 媒體真實位址
        /// </summary>
        public Uri RealUrl { get; set; }

        /// <summary>
        /// 媒體長度(秒)
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 媒體敘述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 影片縮圖網址
        /// </summary>
        public Uri Thumbnail { get; set; }

        /// <summary>
        /// 媒體其他相關屬性
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 剖析結果來源類型
        /// </summary>
        [JsonConverter(typeof(StringTypeConverter))]
        public Type ExtractorType { get; set; }

        public MediaInfo() { }

        /// <summary>
        /// 下載媒體到指定的路徑
        /// </summary>
        /// <param name="SavePath">儲存路徑</param>
        /// <returns></returns>
        public async Task Download(string SavePath) {
            Stream file = await this.GetStreamAsync();

            FileStream output = File.Create(SavePath);

            using (BinaryWriter writer = new BinaryWriter(output)) {
                while (file.Position < file.Length) {
                    writer.Write(file.ReadByte());
                }
            }
        }

        /// <summary>
        /// 取得媒體下載串流
        /// </summary>
        /// <returns>串流</returns>
        public async Task<Stream> GetStreamAsync() {
            return await new HttpClient().GetStreamAsync(this.RealUrl);
        }

        /// <summary>
        /// 取得剖析結果的深層副本
        /// </summary>
        /// <returns></returns>
        public MediaInfo Clone() {
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
