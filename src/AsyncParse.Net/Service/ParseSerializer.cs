using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using AsyncParse.Net.Converters;
using AsyncParse.Net.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AsyncParse.Net.Service
{
    public sealed class ParseSerializer 
    {
        private JsonSerializer _serializer;

        public ParseSerializer()
        {
            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new PointerJsonConverter());
            _serializer.Converters.Add(new GeoPointJsonConverter());
            _serializer.Converters.Add(new ZuluDateTimeConverter());
            _serializer.Converters.Add(new ParseDateJsonConverter());
        }

        public ParseSerializer AddSimpleType<T>() where T : ValueBase, new()
        {
            _serializer.Converters.Add(new ValueBaseJavascriptConverter<T>());
            return this;
        }

        public void Serialize<T>(T target_, StringBuilder sb)
        {
            using (var tw = new StringWriter(sb))
            {
                _serializer.Serialize(tw, target_);
            }
        }

        public T Deserialize<T>(JsonTextReader jsonTextReader_)
        {
            return _serializer.Deserialize<T>(jsonTextReader_);
        }

        public static ParseSerializer Serializer()
        {
            return new ParseSerializer();
        }
    }

    public class ValueBaseJavascriptConverter<T> : JsonConverter
        where T:ValueBase, new() 
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = value as ValueBase;
            if (obj != null)
            {
                writer.WriteValue(obj.Value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            var ret = new T();
            if (reader.TokenType == JsonToken.String)
            {
                ret.Value = reader.Value.ToString();
            }
            return ret;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ValueBase).IsAssignableFrom(objectType);
        }
    }
}