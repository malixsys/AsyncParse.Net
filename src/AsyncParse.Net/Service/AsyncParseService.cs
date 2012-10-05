using System;
using AsyncParse.Net.BuiltIns;

namespace AsyncParse.Net.Service
{
    public class AsyncParseService
    {
        private readonly SecurityKeys _securityKeys;
        public AsyncParseService(string applicationId, string masterKey)
        {
            _securityKeys = new SecurityKeys(applicationId, masterKey);
        }
        public IParseRegistry<T> CreateRegistry<T>(string className)
            where T:ParseObject
        {
            return null;
        }
    }
}