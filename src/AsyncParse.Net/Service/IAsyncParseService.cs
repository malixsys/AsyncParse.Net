using System.Web;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Model;

namespace AsyncParse.Net.Service
{
    public interface IAsyncParseService
    {
        IParseRegistry<T> CreateRegistry<T>(string className_)
            where T : ParseObject;

        ParseCreatedFile SaveFile(HttpPostedFileBase filename);
        bool DeleteFile(string name);
        AsyncCallFailureReason Ping();
    }
}