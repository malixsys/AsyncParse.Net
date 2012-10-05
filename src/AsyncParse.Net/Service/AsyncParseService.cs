using System;
using AsyncParse.Net.BuiltIns;

namespace AsyncParse.Net.Service
{
    public class AsyncParseService
    {
        public IParseRegistry<T> CreateRegistry<T>(string className)
            where T:ParseObject
        {
            return null;
        }
    }
}