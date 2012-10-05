using System;
using System.Net;
using System.Text;

namespace AsyncParse.Net.Service
{
    internal class SecurityKeys
    {
        internal SecurityKeys(string applicationId_, string masterKey_)
        {
            ApplicationId = applicationId_;
            MasterKey = masterKey_;
            Credentials = new NetworkCredential(applicationId_, masterKey_);
        }

        internal NetworkCredential Credentials { get; private set; }

        internal string ApplicationId { get; private set; }
        internal string MasterKey { get; private set; }

        internal string Authorizationheader()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(String.Format("{0}:{1}", ApplicationId, MasterKey)));
        }
    }
}