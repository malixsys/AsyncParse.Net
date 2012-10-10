using System;
using System.IO;
using System.Text;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Model;
using AsyncParse.Net.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Test.AsyncParse.Net.Resources;

namespace Test.AsyncParse.Net
{
    [TestClass]
    public class ParseJsonTests
    {

        [TestMethod]
        public void when_we_serialize_a_TestPoint()
        {
            const string runGuid = "1286fca346034ad9a49fb49b06f9999b";
            var now = new DateTime(2013, 12, 14, 10, 9,11, 456, DateTimeKind.Utc);
            var ser = ParseSerializer
                .Serializer()
                .AddSimpleType<TestPointProp>();
            var source = new TestPoint
                             {
                                 id = "1",
                                 index = 2,
                                 prop = new TestPointProp {Value = "3"},
                                 timestamp = now.ToUniversalTime(),
                                 parseDate = now.ToUniversalTime(),
                                 runId = runGuid,
                                location = new GeoPoint {Latitude = 45.50867M, Longitude = -73.553992M},
                                related = new Pointer {className = "Users", objectId = "deaIBwVQd5"}
                             };
            var json = ToJson(ser, source);
            Assert.AreEqual(
                "{\"related\":{\"__type\":\"Pointer\",\"className\":\"Users\",\"objectId\":\"deaIBwVQd5\"},\"parseDate\":{\"__type\":\"Date\",\"iso\":\"2013-12-14T10:09:11.456Z\"},\"runId\":\"1286fca346034ad9a49fb49b06f9999b\",\"id\":\"1\",\"prop\":\"3\",\"index\":2,\"timestamp\":\"2013-12-14T10:09:11.456Z\",\"location\":{\"__type\":\"GeoPoint\",\"latitude\":45.50867,\"longitude\":-73.553992},\"objectId\":null,\"createdAt\":\"0001-01-01T00:00:00.000Z\",\"updatedAt\":\"0001-01-01T00:00:00.000Z\"}",
                json);


            TestPoint target;
            using (var streamReader = new StringReader(json))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    target = ser.Deserialize<TestPoint>(jsonTextReader);
                    
                }
            }

            Assert.IsNotNull(target);
            Assert.AreEqual("1", target.id);
            Assert.AreEqual(2, target.index);
            Assert.AreEqual("3", target.prop.Value);
            Assert.AreEqual(now, target.timestamp);
            Assert.AreEqual(now, target.parseDate.Value);
            Assert.AreEqual(runGuid, target.runId);
            Assert.AreEqual("deaIBwVQd5", target.related.objectId);
            Assert.AreEqual(45.50867M, target.location.Latitude);
            Assert.AreEqual(-73.553992M, target.location.Longitude);
        }

        private string ToJson(ParseSerializer ser, TestPoint target)
        {
            var sb = new StringBuilder();
            ser.Serialize(target, sb);
            sb = sb.Replace("\\u0027", "'");
            return sb.ToString();
        }
    }

}
