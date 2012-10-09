using System.Web;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Model;

namespace AsyncParse.Net.Service
{
    public interface IParseRegistry<T>
            where T:ParseObject
    {
        bool DeleteAll();
        AsyncCallResult<ParseObject> Add(object value);
        bool SimpleAdd(object value);
        AsyncCallResult<GetListResponse<T>> Find(object criteria = null);
        T Get(string objectId);
        bool Update(T original, object modifieds);
        bool Delete(ParseObject item);
        ParseCreatedFile SaveFile(HttpPostedFileBase filename);
        bool DeleteFile(string name);
    }
}