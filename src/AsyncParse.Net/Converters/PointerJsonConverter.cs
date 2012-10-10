using System;
using AsyncParse.Net.BuiltIns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AsyncParse.Net.Converters
{
    internal class PointerJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = value as Pointer;
            if(obj != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("__type");
                writer.WriteValue("Pointer");
                writer.WritePropertyName("className");
                writer.WriteValue(obj.className);
                writer.WritePropertyName("objectId");
                writer.WriteValue(obj.objectId);
                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var ret = new Pointer();
            ret.className = obj.Value<string>("className");
            ret.objectId = obj.Value<string>("objectId");

            return ret;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Pointer).IsAssignableFrom(objectType);
        }
    }
}