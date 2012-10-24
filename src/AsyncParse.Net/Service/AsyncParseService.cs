using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using AsyncParse.Net.BuiltIns;
using AsyncParse.Net.Extensions;
using AsyncParse.Net.Model;
using Newtonsoft.Json;

namespace AsyncParse.Net.Service
{
    public class AsyncParseService : IAsyncParseService
    {
        private readonly ParseSerializer _serializer;
        private readonly SecurityKeys _securityKeys;
        
        public AsyncParseService(ParseSerializer serializer_, string applicationId, string masterKey)
            : this(new SecurityKeys(applicationId, masterKey))
        {
            _serializer = serializer_;
        }

        public AsyncParseService(string applicationId, string masterKey)
            : this(new SecurityKeys(applicationId, masterKey))
        {
            _serializer = new ParseSerializer();
        }

        private AsyncParseService(SecurityKeys securityKeys_)
        {
            _securityKeys = securityKeys_;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        internal AsyncCallResult<T> Call<T>(string className, object criteria, Func<IAsyncClient, CancellationToken, Task<HttpResponseMessage>> innerCall, JsonSerializerSettings settings = null)
            where T : class
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            using (var client = new AsyncClient(_serializer, className, criteria, _securityKeys.Credentials))
            {
                using (var task = innerCall(client, token))
                {
                    try
                    {
                        if (task.Wait(10000, token) == false)
                        {
                            if (token.CanBeCanceled)
                            {
                                source.Cancel();
                            }
                            return new AsyncCallResult<T>(AsyncCallFailureReason.TimeOut);
                        }
                    }
                    catch (Exception)
                    {
                        return new AsyncCallResult<T>(AsyncCallFailureReason.FailedConnection);
                    }
                    if (task.Result.IsSuccessStatusCode == false)
                    {
                        return new AsyncCallResult<T>(AsyncCallFailureReason.FailedStatusCode);
                    }

                    var content = task.Result.Content.ReadAsStreamAsync();
                    if (content.Wait(250, token) == false)
                    {
                        if (token.CanBeCanceled)
                        {
                            source.Cancel();
                        }
                        return new AsyncCallResult<T>(AsyncCallFailureReason.TimeOut);
                    }
                    using (var streamReader = new StreamReader(content.Result))
                    {
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var obj = _serializer.Deserialize<T>(jsonTextReader);
                            return new AsyncCallResult<T>(AsyncCallFailureReason.None, obj);
                        }
                    }
                }
            }
        }


        public IParseRegistry<T> CreateRegistry<T>(string className_)
            where T : ParseObject
        {
            return new ParseRegistry<T>(this, className_);
        }

        internal bool CallNoReturn(string className, object criteria, Func<IAsyncClient, CancellationToken, Task<HttpResponseMessage>> innerCall)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            using (var client = new AsyncClient(_serializer, className, criteria, _securityKeys.Credentials))
            {
                using (var task = innerCall(client, token))
                {
                    if (task.Wait(30000, token) == false)
                    {
                        if (token.CanBeCanceled)
                        {
                            source.Cancel();
                        }
                        return false;
                    }
                    return task.Result.IsSuccessStatusCode;
                }
            }
        }

        public ParseCreatedFile SaveFile(HttpPostedFileBase filename)
        {
            var result = CreateFile(filename);
            if (result.Failed())
            {
                return null;
            }
            return result.Contents;
        }

        public bool DeleteFile(string name)
        {
            return CallNoReturn("files", name, (c, t) => c.Delete(t));
        }

        internal AsyncCallResult<ParseCreatedFile> CreateFile(HttpPostedFileBase fileName, JsonSerializerSettings settings = null)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            using (var client = new AsyncClient(_securityKeys))
            {
                using (var task = client.SendFile(fileName, token))
                {
                    try
                    {
                        if (task.Wait(300000, token) == false)
                        {
                            if (token.CanBeCanceled)
                            {
                                source.Cancel();
                            }
                            return new AsyncCallResult<ParseCreatedFile>(AsyncCallFailureReason.TimeOut);
                        }
                    }
                    catch (Exception)
                    {
                        return new AsyncCallResult<ParseCreatedFile>(AsyncCallFailureReason.FailedConnection);
                    }
                    if (task.Result.IsSuccessStatusCode == false)
                    {
                        return new AsyncCallResult<ParseCreatedFile>(AsyncCallFailureReason.FailedStatusCode);
                    }
                    var serializer = JsonSerializer.Create(settings ?? new JsonSerializerSettings());
                    var content = task.Result.Content.ReadAsStreamAsync();
                    if (content.Wait(250, token) == false)
                    {
                        if (token.CanBeCanceled)
                        {
                            source.Cancel();
                        }
                        return new AsyncCallResult<ParseCreatedFile>(AsyncCallFailureReason.TimeOut);
                    }
                    using (var streamReader = new StreamReader(content.Result))
                    {
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var obj = serializer.Deserialize<ParseCreatedFile>(jsonTextReader);
                            return new AsyncCallResult<ParseCreatedFile>(AsyncCallFailureReason.None, obj);
                        }
                    }

                }
            }
        }

        public AsyncCallFailureReason Ping()
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            using (var client = new AsyncClient(_securityKeys.Credentials))
            {
                using (var task = client.Ping(token))
                {
                    try
                    {
                        if (task.Wait(5000, token) == false)
                        {
                            if (token.CanBeCanceled)
                            {
                                source.Cancel();
                            }
                            return AsyncCallFailureReason.TimeOut;
                        }
                    }
                    catch (Exception)
                    {
                        return AsyncCallFailureReason.FailedConnection;
                    }
                    if (task.Result.IsSuccessStatusCode == false)
                    {
                        return AsyncCallFailureReason.FailedStatusCode;
                    }
                    var serializer = JsonSerializer.Create(new JsonSerializerSettings());
                    var content = task.Result.Content.ReadAsStreamAsync();
                    if (content.Wait(250, token) == false)
                    {
                        if (token.CanBeCanceled)
                        {
                            source.Cancel();
                        }
                        return AsyncCallFailureReason.TimeOut;
                    }
                    using (var streamReader = new StreamReader(content.Result))
                    {
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var obj = serializer.Deserialize<GetListResponse<ParseObject>>(jsonTextReader);
                            if (obj == null || obj.Results == null)
                            {
                                return AsyncCallFailureReason.ResultsNotFound;
                            }
                            return AsyncCallFailureReason.None;
                        }
                    }
                }
            }
        }
    }
}