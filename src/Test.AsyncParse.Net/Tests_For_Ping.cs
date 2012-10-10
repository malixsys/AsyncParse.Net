using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Extensions;
using AsyncParse.Net.Model;
using AsyncParse.Net.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.AsyncParse.Net.Resources;

namespace Test.AsyncParse.Net
{
    [TestClass]
    public class ParsePingTests
    {
        private AsyncParseService _parse;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var secrets = Secrets.keys_secret.Split('\t');
            _parse = new AsyncParseService(secrets[0], secrets[1]);
        }

        [TestMethod]
        public void when_we_ping_parse()
        {
            var result = _parse.Ping();
            Assert.AreEqual(AsyncCallFailureReason.None, result);
        }
        


    }

}
