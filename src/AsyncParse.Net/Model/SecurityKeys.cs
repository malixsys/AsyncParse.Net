using System;
using System.Net;
using System.Text;

namespace AsyncParse.Net.Model
{
    internal class SecurityKeys
    {
        private readonly string _applicationId;
        private readonly string _masterKey;

        internal SecurityKeys(string applicationId_, string masterKey_)
        {
            _applicationId = applicationId_;
            _masterKey = masterKey_;
            Credentials = new NetworkCredential(applicationId_, masterKey_);
            Authorizationheader = Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", _applicationId, _masterKey)));
        }

        internal NetworkCredential Credentials { get; private set; }

        internal string Authorizationheader { get; private set; }
    }
}