using System;
using System.Collections.Generic;
using System.Linq;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Converters;
using AsyncParse.Net.Extensions;
using AsyncParse.Net.Model;
using AsyncParse.Net.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.AsyncParse.Net.Resources;

namespace Test.AsyncParse.Net
{
    [TestClass]
    public class ParseRegistryTests
    {
        private AsyncParseService _parse;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var secrets = Secrets.Keys.Split('\t');
            var ser = ParseSerializer
                .Serializer()
                .AddSimpleType<TestPointProp>();
            _parse = new AsyncParseService(ser, secrets[0], secrets[1]);
        }


        [TestMethod]
        public void when_we_use_parse()
        {
            using (var tester = new AsyncParseSelfCleaningTester<TestPoint>(_parse, "TestPoints"))
            {
                var model = tester.Model;
                var runGuid = Guid.NewGuid().ToString("N");
                var now = ZuluDateTimeConverter.FromZuluString(ZuluDateTimeConverter.ToZuluString(DateTime.Now.ToUniversalTime()));

                //add one
                var guids = new List<string>(new []
                                {
                                    Guid.NewGuid().ToString("N"),
                                    Guid.NewGuid().ToString("N"),
                                    Guid.NewGuid().ToString("N")
                                });

                var props = new List<string>( new []
                                 {
                                     Guid.NewGuid().ToString("N"),
                                     Guid.NewGuid().ToString("N")
                                 });

                var prop1 = new TestPointProp {Value = props[0]};
                var alpha = tester.Add(new
                                           {
                                               runId = runGuid,
                                               id = guids[0],
                                               prop = prop1,
                                               timestamp = now,
                                               index = 1,
                                               parseDate = new ParseDate {Value = now},
                                               location = new GeoPoint {Latitude = 45.50867M, Longitude = -73.553992M},
                                               related = new Pointer {className = "Users", objectId = "deaIBwVQd5"}
                                           });

                var beta = tester.Add(new
                                          {
                                              runId = runGuid,
                                              id = guids[1],
                                              prop = prop1,
                                              timestamp = now,
                                              index = 2
                                          });

                var prop2 = new TestPointProp { Value = props[1] };
                tester.SimpleAdd(new
                                           {
                                               runId = runGuid,
                                               id = guids[2],
                                               prop = prop2,
                                               timestamp = now,
                                               index = 3
                                           });

                //retrieve it back by objectId
                var second = model.Get(alpha.Contents.objectId);
                AssertThat.IsNotNull(second);
                AssertThat.AreEqual(runGuid, second.runId);
                AssertThat.AreEqual("deaIBwVQd5", second.related.objectId);
                AssertThat.AreEqual(alpha.Contents.objectId, second.objectId);

                //get all of them
                var all = model.Find();
                AssertThat.IsNotNull(all);
                AssertThat.IsTrue(all.FailureReason == AsyncCallFailureReason.None);
                AssertThat.IsTrue(all.Contents.Count >= 3);
                var third = all.Contents.Results.FirstOrDefault(i => i.prop.Value == prop2.Value);
                AssertThat.IsNotNull(third);
                if (third != null)
                {
                    AssertThat.AreEqual(runGuid, third.runId);
                    AssertThat.AreEqual(guids[2], third.id);
                    tester.AddObject(third);
                }

                //retrieve it back by id
                var some = model.Find(new
                                          {
                                              id = guids[1]
                                          });

                AssertThat.IsNotNull(some);
                AssertThat.IsTrue(some.FailureReason == AsyncCallFailureReason.None);
                AssertThat.AreEqual(1, some.Contents.Count);
                var fourth = some.FirstOrNull();
                AssertThat.IsNotNull(fourth);
                AssertThat.AreEqual(runGuid, fourth.runId);
                AssertThat.AreEqual(beta.Contents.objectId, fourth.objectId);
                AssertThat.AreEqual(2, fourth.index);

                //retrieve some back by prop
                some = model.Find(new
                                      {
                                          prop = prop1
                                      });
                AssertThat.IsNotNull(some);
                AssertThat.IsTrue(some.FailureReason == AsyncCallFailureReason.None);
                AssertThat.AreEqual(2, some.Contents.Count);
                var fifth = some.FirstOrNull();
                AssertThat.IsNotNull(fifth);
                AssertThat.AreEqual(runGuid, fifth.runId);
                AssertThat.AreEqual(prop1.Value, fifth.prop.Value);
                AssertThat.AreEqual(now, fifth.timestamp);
                AssertThat.IsTrue(alpha.Contents.objectId == fifth.objectId || beta.Contents.objectId == fifth.objectId);

                //modify one
                model.Update(fourth, new {index = 5});
                var sixth = model.Get(fourth.objectId);
                AssertThat.IsNotNull(sixth);
                AssertThat.AreEqual(runGuid, sixth.runId);
                AssertThat.AreEqual(fourth.objectId, sixth.objectId);
                AssertThat.AreEqual(5, sixth.index);
                AssertThat.AreEqual(guids[1], sixth.id);

                tester.Clean();

                all = model.Find(new
                {
                    runId = runGuid,
                });
                Assert.IsTrue(all.IsEmpty());
                Assert.IsFalse(all.HasResults());
            }

        }
        

    }

}
