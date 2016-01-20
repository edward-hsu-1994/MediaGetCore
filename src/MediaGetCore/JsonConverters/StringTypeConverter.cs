using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace MediaGetCore.JsonConverters {
    public class StringTypeConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(Type);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return typeof(IExtractor).GetTypeInfo().Assembly.GetType(reader.Value as string);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            Type type = value as Type;
            writer.WriteValue(type?.Name);
        }
    }
}
