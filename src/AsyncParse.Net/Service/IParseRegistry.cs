using System.Web;
using AsyncParse.Net.BuiltIns;

namespace AsyncParse.Net.Service
{
    public interface IParseRegistry<T>
            where T:ParseObject
    {
        ParseCreatedFile SaveFile(HttpPostedFileBase filename);
        bool DeleteFile(string name);
    }
}