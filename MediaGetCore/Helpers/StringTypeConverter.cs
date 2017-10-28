using System;
using System.Reflection;
using Newtonsoft.Json;

namespace MediaGetCore.Helpers {
    /// <summary>
    /// 針對Json.net之<see cref="string"/>與<see cref="Type"/>互相轉換之轉換器
    /// </summary>
    internal class StringTypeConverter : JsonConverter {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType) => objectType == typeof(Type);

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return typeof(IExtractor).GetTypeInfo().Assembly.GetType(reader.Value as string);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            Type type = value as Type;
            writer.WriteValue(type?.Name);
        }
    }
}
