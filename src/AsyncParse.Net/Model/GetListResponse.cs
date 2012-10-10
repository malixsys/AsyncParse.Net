using System.Collections.Generic;
using AsyncParse.Net.BuiltIns;

namespace AsyncParse.Net.Model
{
// ReSharper disable ClassNeverInstantiated.Global
    public class GetListResponse<T>
        where T:ParseObject
// ReSharper restore ClassNeverInstantiated.Global
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public List<T> Results { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        public int Count
        {
            get { return Results == null ? 0 : Results.Count; }
        }

        public T this[int i_]
        {
            get { return Results == null || Results.Count <= i_ ? default(T) : Results[i_]; }
        }
    }
}