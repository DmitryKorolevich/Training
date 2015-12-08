using System.IO;
using System.Net;
using System.Threading.Tasks;
using Authorize.Net.AIM.Requests;
using Authorize.Net.AIM.Responses;
using Authorize.Net.Utility;

namespace Authorize.Net.AIM
{
    public enum ServiceMode
    {
        Test, // Test mode points to certification test systems
        Live
    }

    public class Gateway : IGateway
    {
        public const string TEST_URL = "https://test.authorize.net/gateway/transact.dll";
        public const string LIVE_URL = "https://secure2.authorize.net/gateway/transact.dll";

        public Gateway(string apiLogin, string transactionKey, bool testMode)
        {
            ApiLogin = apiLogin;
            TransactionKey = transactionKey;
            TestMode = testMode;
        }

        public Gateway(string apiLogin, string transactionKey) : this(apiLogin, transactionKey, true)
        {
        }

        public bool TestMode { get; set; }


        public string ApiLogin { get; set; }
        public string TransactionKey { get; set; }

        public Task<IGatewayResponse> Send(IGatewayRequest request)
        {
            return Send(request, null);
        }

        public virtual async Task<IGatewayResponse> Send(IGatewayRequest request, string description)
        {
            var serviceUrl = TEST_URL;
            if (!TestMode)
                serviceUrl = LIVE_URL;

            LoadAuthorization(request);
            if (string.IsNullOrEmpty(request.Description))
                request.Queue(ApiFields.Description, description);
#if debug          
            request.DuplicateWindow = "0";
#endif
            var response = await SendRequest(serviceUrl, request);

            return DecideResponse(response.Split('|'));
        }

        protected void LoadAuthorization(IGatewayRequest request)
        {
            request.Queue(ApiFields.ApiLogin, ApiLogin);
            request.Queue(ApiFields.TransactionKey, TransactionKey);
        }

        protected async Task<string> SendRequest(string serviceUrl, IGatewayRequest request)
        {
            var postData = request.ToPostString();
            //override the local cert policy - this is for Mono ONLY
            //ServicePointManager.CertificatePolicy = new PolicyOverride();

            var webRequest = (HttpWebRequest) WebRequest.Create(serviceUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // post data is sent as a stream
            using (var myWriter = new StreamWriter(await webRequest.GetRequestStreamAsync()))
            {
                await myWriter.WriteAsync(postData);
            }
            // returned values are returned as a stream, then read into a string
            var response = await webRequest.GetResponseAsync();
            using (var responseStream = new StreamReader(response.GetResponseStream()))
            {
                // the response string is broken into an array
                // The split character specified here must match the delimiting character specified above
                return responseStream.ReadToEnd();
            }
        }


        /// <summary>
        ///     Decides the response.
        /// </summary>
        /// <param name="rawResponse">The raw response.</param>
        /// <returns></returns>
        public IGatewayResponse DecideResponse(string[] rawResponse)
        {
            if (rawResponse.Length == 1)
                throw new InvalidDataException("There was an error returned from AuthorizeNet: " + rawResponse[0] +
                                               "; this usually means your data sent along was incorrect. Please recheck that all dates and amounts are formatted correctly");

            return new GatewayResponse(rawResponse);
        }
    }
}