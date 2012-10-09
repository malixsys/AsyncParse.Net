using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Model;
using AsyncParse.Net.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.AsyncParse.Net.Resources;

namespace Test.AsyncParse.Net
{
    [TestClass]
    public class ParseTests
    {
        private AsyncParseService _parse;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var secrets = Secrets.keys_secret.Split('\t');
            var serializer = ParseSerializer.Serializer(typeof(GeoPoint));
            _parse = new AsyncParseService(serializer, secrets[0], secrets[1]);
        }

        [TestMethod]
        public void when_we_ping_parse()
        {
            var result = _parse.Ping();
            Assert.AreEqual(AsyncCallFailureReason.None, result);
        }

        [TestMethod]
        public void when_we_use_parse()
        {
            using (var tester = new AsyncParseSelfCleaningTester<TestPoint>(_parse, "TestPoints"))
            {
                var model = tester.Model;

                //add one
                var guids = new List<string>(new []
                                {
                                    Guid.NewGuid().ToString("N"),
                                    Guid.NewGuid().ToString("N"),
                                    Guid.NewGuid().ToString("N")
                                });

                var colors = new List<string>( new []
                                 {
                                     Guid.NewGuid().ToString("N"),
                                     Guid.NewGuid().ToString("N")
                                 });

                var red = colors[0];
                var alpha = tester.Add(new
                                           {
                                               id = guids[0],
                                               color = red,
                                               index = 1
                                           });

                var beta = tester.Add(new
                                          {
                                              id = guids[1],
                                              color = red,
                                              index = 2
                                          });

                var blue = colors[1];
                tester.SimpleAdd(new
                                           {
                                               id = guids[2],
                                               color = blue,
                                               index = 3
                                           });

                //retrieve it back by objectId
                var second = model.Get(alpha.Contents.objectId);
                AssertThat.IsNotNull(second);
                AssertThat.AreEqual(alpha.Contents.objectId, second.objectId);

                //get all of them
                var all = model.Find();
                AssertThat.IsNotNull(all);
                AssertThat.IsTrue(all.FailureReason == AsyncCallFailureReason.None);
                AssertThat.IsTrue(all.Contents.Count >= 3);
                var third = all.Contents.Results.FirstOrDefault(i => i.color == blue);
                AssertThat.IsNotNull(third);
                if (third != null)
                {
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
                var fourth = some.Contents[0];
                AssertThat.IsNotNull(fourth);
                AssertThat.AreEqual(beta.Contents.objectId, fourth.objectId);
                AssertThat.AreEqual(2, fourth.index);

                //retrieve some back by color
                some = model.Find(new
                                      {
                                          color = red
                                      });
                AssertThat.IsNotNull(some);
                AssertThat.IsTrue(some.FailureReason == AsyncCallFailureReason.None);
                AssertThat.AreEqual(2, some.Contents.Count);
                var fifth = some.Contents[0];
                AssertThat.IsNotNull(fifth);
                AssertThat.IsTrue(alpha.Contents.objectId == fifth.objectId || beta.Contents.objectId == fifth.objectId);

                //modify one
                model.Update(fourth, new {index = 5});
                var sixth = model.Get(fourth.objectId);
                AssertThat.IsNotNull(sixth);
                AssertThat.AreEqual(fourth.objectId, sixth.objectId);
                AssertThat.AreEqual(5, sixth.index);
                AssertThat.AreEqual(sixth.id, guids[1]);

                tester.Clean();
            }

        }
        
        [TestMethod]
        public void when_we_upload_dowload_and_delete_a_file()
        {
            var registry = _parse.CreateRegistry<ParseObject>("TestFileObjects");
            using (var testFile = CreateTestFile())
            {
                var res = registry.SaveFile(testFile);
                Assert.IsNotNull(res);
                Assert.IsNotNull(res.name);
                Assert.IsNotNull(res.url);
                using (var client = new HttpClient())
                {
                    var code1 = client.GetAsync(res.url, HttpCompletionOption.ResponseHeadersRead).Result.StatusCode;
                    Assert.AreEqual(HttpStatusCode.OK, code1);
                    registry.DeleteFile(res.name);
                }
            }
        }

        private static HttpPostedFileBaseFake CreateTestFile()
        {
            var file = new HttpPostedFileBaseFake();
            return file;
        }

    }

    internal class HttpPostedFileBaseFake : HttpPostedFileBase, IDisposable
    {
        private readonly string _filename;
        private FileStream _stream;
        private readonly long _length;

        public HttpPostedFileBaseFake()
        {
            var path = Path.GetTempPath();
            _filename = Path.Combine(path, "test.jpg");
            var codec = GetEncoderInfo("image/jpeg");
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(Encoder.Quality, 99L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            Files.Parse.Save(_filename, codec, myEncoderParameters);
            _length = new FileInfo(_filename).Length;
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        public override string ContentType
        {
            get { return "image/jpeg"; }
        }

        public override int ContentLength
        {
            get { return (int)_length; }
        }

        public override string FileName
        {
            get { return _filename; }
        }

        public override Stream InputStream
        {
            get
            {
                _stream = File.OpenRead(_filename);
                return _stream;
            }
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                try
                {
                    var s = _stream;
                    _stream = null;
                    s.Dispose();
                }
                catch (Exception ex)
                {
                    ex.ToString();//ignore
                }
            }
            try
            {
                File.Delete(_filename);
            }
            catch (Exception ex)
            {
                ex.ToString();//ignore
            }
        }
    }

}
