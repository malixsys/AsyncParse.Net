using System.Linq;
using System.Web;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Extensions;
using AsyncParse.Net.Model;

namespace AsyncParse.Net.Service
{
    internal class ParseRegistry<T> : IParseRegistry<T> 
        where T : ParseObject
    {
        private readonly AsyncParseService _parse;
        private readonly string _className;

        public ParseRegistry(AsyncParseService parse, string className)
        {
            _parse = parse;
            _className = className;
        }

        public bool DeleteAll()
        {
            var result = Find();
            if (result.Failed())
                return false;
            if( result.Contents.Count == 0)
                return true;

            return result.Contents.Results
                .All(this.Delete);
        }

        public AsyncCallResult<ParseObject> Add(object value) 
        {
            return _parse.Call<ParseObject>(_className, value, (c,t) => c.Post(t));
        }

        public bool SimpleAdd(object value)
        {
            return _parse.CallNoReturn(_className, value, (c, t) => c.Post(t));
        }


        public AsyncCallResult<GetListResponse<T>> Find(object criteria = null)
        {
            return _parse.Call<GetListResponse<T>>(_className, criteria, (c, t) => c.Get(t));
        }

        public T Get(string objectId)
        {
            var result = _parse.Call<T>(_className, objectId, (c, t) => c.Get(t));
            if (result.Failed())
            {
                return null;
            }
            return result.Contents;
        }

        public bool Update(T original, object modifieds)
        {
            var result = _parse.Call<ParseObject>(_className, original.objectId, (c, t) => c.Put(modifieds, t));
            if (result.Failed())
            {
                return false;
            }
            original.updatedAt = result.Contents.updatedAt;
            return true;
        }

        public bool Delete(ParseObject original)
        {
            return _parse.CallNoReturn(_className, original.objectId, (c, t) => c.Delete(t));
        }

    }
}