using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using AsyncParse.Net.BuiltIns;

namespace AsyncParse.Net.Service
{
    public sealed class ParseSerializer : JavaScriptSerializer
    {
        public const string ZULU_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public ParseSerializer(IEnumerable<Type> types)
            : base(new CustomTypeResolver(types))
        {
            RegisterConverters(new JavaScriptConverter[] { new UriStringJavaScriptConverter(typeof(DateTime)), new GeoPointJavascriptConverter(), new PointerJavascriptConverter() });
        }

        public static ParseSerializer Serializer(params Type[] types)
        {
            return new ParseSerializer(types);
        }

        public void RegisterSimpleConverters(params Type[] types)
        {
            RegisterConverters(new JavaScriptConverter[] { new UriStringJavaScriptConverter(types) });
        }
    }

    class PointerJavascriptConverter : JavaScriptConverter
    {
        private readonly Type[] _types = new[] { typeof(Pointer) };
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new Pointer
            {
                className = (string)dictionary["className"],
                objectId = (string)dictionary["objectId"]
            };
        }


        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var dic = new Dictionary<string, object>();
            var gp = obj as Pointer;
            if (gp != null)
            {
                dic.Add("className", gp.className);
                dic.Add("objectId", gp.objectId);
            }
            return dic;
        }


        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _types;
            }
        }


    }

    class GeoPointJavascriptConverter : JavaScriptConverter
    {
        private readonly Type[] _types = new[] { typeof(GeoPoint) };
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return new GeoPoint
            {
                Latitude = (Decimal)dictionary["latitude"],
                Longitude = (Decimal)dictionary["longitude"]
            };
        }


        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var dic = new Dictionary<string, object>();
            var gp = obj as GeoPoint;
            if (gp != null)
            {
                dic.Add("latitude", gp.Latitude);
                dic.Add("longitude", gp.Longitude);
            }
            return dic;
        }


        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return _types;
            }
        }


    }

    public class CustomTypeResolver : JavaScriptTypeResolver
    {
        private readonly Dictionary<string, Type> _types;

        public CustomTypeResolver(IEnumerable<Type> types_)
        {
            _types = new Dictionary<string, Type>();
            foreach (Type type in types_)
            {
                _types.Add(type.Name, type);
            }
        }

        public override string ResolveTypeId(Type type)
        {
            if (_types.ContainsKey(type.Name))
            {
                return type.Name;
            }
            return null;
        }
        public override Type ResolveType(string id)
        {
            //if (id == "GeoPoint")
            if (_types.ContainsKey(id))
            {
                return _types[id];
            }
            throw new ArgumentOutOfRangeException("id", id, "Type unknown");
        }
    }


    //        return new Dictionary<string, object> { { "", ((DateTime)obj).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fZ") } }; // customize this




    class UriStringJavaScriptConverter : JavaScriptConverter
    {
        private readonly Type[] _types;

        public UriStringJavaScriptConverter(params Type[] types)
        {
            _types = types;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            if (obj == null)
            {
                return new CustomString(null);
            }
            if (obj is DateTime)
            {
                return new CustomString(((DateTime)obj).ToUniversalTime().ToString(ParseSerializer.ZULU_DATE_FORMAT));
            }
            if (obj is ValueBase)
            {
                return new CustomString(((ValueBase)obj).Value);
            }
            return new CustomString(obj.ToString());
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return _types; }
        }

    }

    class CustomString : Uri, IDictionary<string, object>
    {
        private readonly string _str;

        public CustomString(string str)
            : base(str, UriKind.Relative)
        {
            _str = str;
        }

        public override string ToString()
        {
            return _str;
        }
        public override bool Equals(object obj)
        {
            return _str.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _str.GetHashCode();
        }
        void IDictionary<string, object>.Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { throw new NotImplementedException(); }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            throw new NotImplementedException();
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { throw new NotImplementedException(); }
        }

        object IDictionary<string, object>.this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
            }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

}