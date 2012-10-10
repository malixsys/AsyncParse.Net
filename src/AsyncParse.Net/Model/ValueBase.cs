using System.Collections.Generic;

namespace AsyncParse.Net.Model
{
    public class ValueBase
    {
        public string Value { get; set; }
        public override string ToString()
        {
            return Value;
        }

        public void ToDict(IDictionary<string, object> dictionary_)
        {
            dictionary_.Add("",Value);    
        }

        public static T FromDictionary<T>(IDictionary<string, object> dictionary_)
            where T:ValueBase, new()
        {
            var ret = new T();
            return ret;
        }
    }
}