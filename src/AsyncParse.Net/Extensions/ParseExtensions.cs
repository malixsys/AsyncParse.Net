using System.Text;
using System.Web.Script.Serialization;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Model;
using AsyncParse.Net.Service;

namespace AsyncParse.Net.Extensions
{
    public static class ParseExtensions
    {
        public static bool Failed<T>(this AsyncCallResult<T> result) where T : class
        {
            return result == null || result.FailureReason != AsyncCallFailureReason.None;
        }

        public static string ToJson(this object data_, ParseSerializer ser)
        {
            var sb = new StringBuilder();
            ser.Serialize(data_, sb);
            sb = sb.Replace("\\u0027", "'");
            return sb.ToString();
        }

        public static T FirstOrNull<T>(this AsyncCallResult<GetListResponse<T>> result_)
            where T : ParseObject
        {
            if (result_.HasResults())
            {
                return result_.Contents[0];
            }
            return null;
        }


        public static bool IsEmpty<T>(this AsyncCallResult<GetListResponse<T>> result_)
            where T : ParseObject
        {
            return result_.Failed() || result_.Contents.Count == 0;
        }

        public static bool HasResults<T>(this AsyncCallResult<GetListResponse<T>> result_)
            where T : ParseObject
        {
            return result_.Failed() == false && result_.Contents.Count > 0;
        }
    }
}