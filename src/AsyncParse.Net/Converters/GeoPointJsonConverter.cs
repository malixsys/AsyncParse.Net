using System;
using AsyncParse.Net.BuiltIns;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AsyncParse.Net.Converters
{
    class GeoPointJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = value as GeoPoint;
            if (obj != null)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("__type");
                writer.WriteValue("GeoPoint");
                writer.WritePropertyName("latitude");
                writer.WriteValue(obj.Latitude);
                writer.WritePropertyName("longitude");
                writer.WriteValue(obj.Longitude);
                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var ret = new GeoPoint();
            ret.Latitude = obj.Value<decimal>("latitude");
            ret.Longitude = obj.Value<decimal>("longitude");

            return ret;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(GeoPoint).IsAssignableFrom(objectType);
        }
    }
}