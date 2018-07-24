using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XWidget.Utilities;

namespace XWidget.Web.Jwt {
    public class JwtTimeConvert : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            try {
                return DateTimeUtility.FromUnixTimestamp((long)reader.Value);
            } catch {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) return;
            writer.WriteValue(DateTimeUtility.ToUnixTimestamp((DateTime)value));
        }
    }
}
