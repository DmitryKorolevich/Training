using System;
using System.Reflection;
using System.Collections.Generic;
using SendGrid.CSharp.HTTP.Client;

namespace SendGrid
{
    public class SendGridApiClient
    {
        private string _apiKey;
        private Uri _baseUri;
        public string Version { get; }
        public dynamic Client { get; }

        private enum Methods
        {
            Get,
            Post,
            Patch,
            Delete
        }

        /// <summary>
        ///     Create a client that connects to the SendGrid Web API
        /// </summary>
        /// <param name="apiKey">Your SendGrid API Key</param>
        /// <param name="baseUri">Base SendGrid API Uri</param>
        /// <param name="version">API Version (v2 or v3)</param>
        public SendGridApiClient(string apiKey, string baseUri = "https://api.sendgrid.com", string version = "v3")
        {
            _baseUri = new Uri(baseUri);
            _apiKey = apiKey;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var requestHeaders = new Dictionary<string, string>
            {
                {"Authorization", "Bearer " + apiKey},
                {"Content-Type", "application/json"},
                {"User-Agent", "sendgrid/" + Version + " csharp"},
                {"Accept", "application/json"}
            };
            Client = new Client(host: baseUri, requestHeaders: requestHeaders, version: version);
        }
    }
}