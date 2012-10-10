using System;
using System.Globalization;
using Newtonsoft.Json;

namespace AsyncParse.Net.Converters
{
    public class ZuluDateTimeConverter : JsonConverter
    {
        public const string ZULU_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is DateTime)
            {
                var obj = (DateTime)value;
                writer.WriteValue(ToZuluString(obj));
            }
        }

        public static string ToZuluString(DateTime obj)
        {
            return obj.ToString(ZULU_DATE_FORMAT);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.String)
            {
                var str = reader.Value.ToString();
                return FromZuluString(str);
            }
            if (reader.TokenType == JsonToken.Date)
            {
                var val = reader.Value;
                if (val is DateTime)
                {
                    return val;
                }
            }

            return new DateTime();
        }

        public static DateTime FromZuluString(string str)
        {
            return DateTime.ParseExact(str, ZULU_DATE_FORMAT, CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeUniversal |
                                       DateTimeStyles.AdjustToUniversal);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType);
        }
    }
}