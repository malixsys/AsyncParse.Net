using System;
using AsyncParse.Net.BuiltIns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AsyncParse.Net.Converters
{
    internal class ParseDateJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = value as ParseDate;
            if(obj != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("__type");
                writer.WriteValue("Date");
                writer.WritePropertyName("iso");
                writer.WriteValue(ZuluDateTimeConverter.ToZuluString(obj.Value));
                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var ret = new ParseDate
                          {
                              Value = obj.Value<DateTime>("iso")
                          };

            return ret;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ParseDate).IsAssignableFrom(objectType);
        }
    }
}