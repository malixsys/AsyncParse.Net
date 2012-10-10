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
    public class ParseFilesTests
    {
        private AsyncParseService _parse;

        [TestInitialize]
        public void MyTestInitialize()
        {
            var secrets = Secrets.keys_secret.Split('\t');
            _parse = new AsyncParseService(secrets[0], secrets[1]);
        }

        [TestMethod]
        public void when_we_upload_dowload_and_delete_a_file()
        {
            using (var testFile = CreateTestFile())
            {
                var res = _parse.SaveFile(testFile);
                Assert.IsNotNull(res);
                Assert.IsNotNull(res.name);
                Assert.IsNotNull(res.url);
                using (var client = new HttpClient())
                {
                    var code1 = client.GetAsync(res.url, HttpCompletionOption.ResponseHeadersRead).Result.StatusCode;
                    Assert.AreEqual(HttpStatusCode.OK, code1);
                    _parse.DeleteFile(res.name);
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
