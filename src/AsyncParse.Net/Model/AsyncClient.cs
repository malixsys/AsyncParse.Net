using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using AsyncParse.Net.Extensions;
using AsyncParse.Net.Service;

namespace AsyncParse.Net.Model
{
    public class AsyncClient : IDisposable
    {
        private const string PARSE_ROOT_URL = "https://api.parse.com/1";
        private readonly string _className;
        private readonly object _criteria;
        private HttpClientHandler _handler;
        private HttpClient _client;
        private bool _disposed;
        private readonly JavaScriptSerializer _serializer;
        private SecurityKeys _SecurityKeys;

        public AsyncClient(JavaScriptSerializer serializer, string className, object criteria, ICredentials credentials)
        {
            _serializer = serializer;
            _className = className;
            _criteria = criteria;
            _handler = new HttpClientHandler { Credentials = credentials };
            _client = new HttpClient(_handler);
        }

        public AsyncClient(ICredentials credentials)
        {
            _handler = new HttpClientHandler { Credentials = credentials };
            _client = new HttpClient(_handler);
        }

        /// <summary>
        /// used for files
        /// </summary>
        /// <param name="credentials"></param>
        internal AsyncClient(SecurityKeys securityKeys)
        {
            var handler = new HttpClientHandler { PreAuthenticate = false, UseDefaultCredentials = false, MaxRequestContentBufferSize = 5000000 };
            _client = new HttpClient(handler);
            _SecurityKeys = securityKeys;
        }

        private string GetUrlEncodedCriteria(object data)
        {
            string json = data.ToJson(_serializer);
            return HttpUtility.UrlEncode(json);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_client != null)
                {
                    var hc = _client;
                    _client = null;
                    hc.Dispose();
                }
                if (_handler != null)
                {
                    var hh = _handler;
                    _handler = null;
                    hh.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion IDisposable Members

        public Task<HttpResponseMessage> Get(CancellationToken token)
        {
            string uri;
            if (_criteria == null)
            {
                uri = string.Format(PARSE_ROOT_URL + "/classes/{0}", _className);
            }
            else if (_criteria is string)
            {
                uri = string.Format(PARSE_ROOT_URL + "/classes/{0}/{1}", _className, _criteria);
            }
            else
            {
                var query = GetUrlEncodedCriteria(_criteria);
                uri = string.Format(PARSE_ROOT_URL + "/classes/{0}?where={1}", _className, query);
            }
            return _client.GetAsync(new Uri(uri), HttpCompletionOption.ResponseContentRead, token);
        }

        public Task<HttpResponseMessage> Delete(CancellationToken token)
        {
            var uri = new Uri(string.Format(PARSE_ROOT_URL + (_className.ToLowerInvariant() == "files" ? "/{0}/{1}" : "/classes/{0}/{1}"), _className, _criteria));
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            return _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        }


        public Task<HttpResponseMessage> Post(CancellationToken token)
        {
            var uri = new Uri(string.Format(PARSE_ROOT_URL + "/classes/{0}", _className));
            string json = _criteria.ToJson(_serializer);
            var content = new StringContent(json);
            return _client.PostAsync(uri, content, token);
        }

        public Task<HttpResponseMessage> Put(object modifieds, CancellationToken token)
        {
            var uri = new Uri(string.Format(PARSE_ROOT_URL + "/classes/{0}/{1}", _className, _criteria));
            string json = modifieds.ToJson(_serializer);
            var content = new StringContent(json);
            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = content
            };
            return _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
        }

        public Task<HttpResponseMessage> SendFile(HttpPostedFileBase file, CancellationToken token)
        {
            var uri = new Uri(PARSE_ROOT_URL + "/files/" + Path.GetFileName(file.FileName));
            //Byte[] str = new byte[file.ContentLength];
            //file.InputStream.Read(str, 0, file.ContentLength);
            var content = new StreamContent(file.InputStream);
            content.Headers.ContentLength = file.ContentLength;
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = content;
            //request.Headers.Add("X-Parse-Application-Id", _SecurityKeys.ApplicationId);
            //request.Headers.Add("X-Parse-REST-API-Key", _SecurityKeys.RestApiKey);
            request.Headers.Add("Authorization", "Basic " + _SecurityKeys.Authorizationheader());
            return _client.SendAsync(request, HttpCompletionOption.ResponseContentRead, token);
            /*WebRequest webRequest = WebRequest.Create(uri);
            webRequest.ContentType = contentType;
            SetFileHeaders(webRequest);
            Dictionary<String, String> returnObject = JsonConvert.DeserializeObject<Dictionary<String, String>>(PostFileToParse(parseFile.LocalPath, parseFile.ContentType));
            parseFile.Url = returnObject["url"];
            parseFile.Name = returnObject["name"];
            return parseFile;*/
        }

        public Task<HttpResponseMessage> Ping(CancellationToken token)
        {
            return _client.GetAsync(new Uri(PARSE_ROOT_URL + "/users"), HttpCompletionOption.ResponseContentRead, token);
        }
    }
}