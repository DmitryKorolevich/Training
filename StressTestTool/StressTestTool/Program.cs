using Newtonsoft.Json;
using StressTestTool.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StressTestTool
{
    class Program
    {
        static Dictionary<string, RequestConfiguration> requestConfigurations;
        static SessionConfiguration sessionConfiguration;
        

        static void Main(string[] args)
        {
            BuildRequestsConfiguration();
            BuildSessionConfiguration();

            var currentRequestConfig = requestConfigurations["h"];
            var request = ConfigureRequest(currentRequestConfig, sessionConfiguration);

            Console.WriteLine("Send request");

            var watcher = Stopwatch.StartNew();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            watcher.Stop();
            var elapsedMilliseconds = watcher.ElapsedMilliseconds;

            Console.WriteLine("Response received");

            var content = ReadResponse(response);

            Console.ReadKey();
        }

        static void BuildSessionConfiguration()
        {
            sessionConfiguration = new SessionConfiguration()
            {
                AdminCookieName = "VitalChoice.Admin",
                AdminCookieValue = "CfDJ8HiEOqEffqxPi_YCd3htiEbwAL7ZqLfXnBe0fia-X61-CJDZqHWBgve0BtS4GbL7iF5BzoAZZki-AkieRqvdVaLnih9N-mR8giKJgP9NBbZGlCfJbPraDxspm74OyzfwBwPU6UwtbcBI2v6wWQjAQihXMPAesf77BdIileFZJD0SVZuhslT4tfmCmOoLXzlf5oH7ivpWsKWv5tzT60A_Nbczqg-40kyin5ZCqMvMm8rmHRmaF5TvyyBUuxdqVlknUHwRQ6V0qB0nigENnfPStdY3AO7AJwl2PpTRe_SjPo1J8JAPbeGexiCxNVHcG1f7hWxJn1Nb_rZekdc6JMFK5a4e09vftbpbKgbFrEh0b1Q9PAWAr7nwG8cxOzO1ipggooZvgSoO0i2lK6-ieaafJWJV3kF_qzUdl2hVf0SQFFEwFuZD8MDhZ87yh_paShhRZuQMwzPHg01QPnb8Xbpd6UotXMg6RbkTj76jnc2lygHO53QYOpNMQ4ECsKMI35-0vWyKt5SOdWe6_zUxiKQ2IwBuyTFm2uISZv1JBIySNNPhgafXvFCrbq22oZ7B6oZWTMJlyvKI4FVi_yX8kgW_mpWpJ5TWoZxxBnjbXddFmuOuVjyLO2y7vwhaPunWgPH8_qLandDAC6bHyM2nOP0Smo7xF3HcLbOexmNfuDE48rSoIsknrrF06kewc4-2WdKfr4zCiQ-ZNavMuK5Haoo1fj3F9obWpIP4Z1YCrvUPmvneYoBjOxOk349ou3tsduTpmq-p7jiG-0kFoYjuWezqLwEBIit79aho0FqdgZY7RMSSzgZJK_L0KXveVchxTPzdDeoJHNvskpeR3YmHFOoCC7Q8enLHJqAs1E_QcFWHY1bMrUv1r5U-1iZ8mA-HgD0yjWGw2iQpHVsgDj903CSLueZ2SMcHiLAW0lD1_fX6LauQrNxfx4YYYd49fDJamrVcPdYNzbg7Fs4NX36E8bIE2if4k4EPgwHPu5RD2_leNppePp-tG2gzuI_JXuyX-NYMvfpmkyOZj_w8IkHOWk77MpG-J35QPYKFjdnrGuNuCOAlADA8PRF-qETHAcv4jOPkvTPiPTzyIYUJ0taszx1R1BYOOFgdvotLz3rDu0CIdpbAbcHArOCz4nNaWejGoyNvRWSt9nVB-2IJ5mKDpnmPGmlkO3lyKkc4gjZ2s73RgLkFxjdI8W0rKiq_qYARtTtareipRaAZwNEQnLW5oc12KsCWKZ7Zm-5G17i0aiIjuLY69FKQfaP9hzGPqy6gCFkZs0YPKhPtsBBWXeRKIKFHbD7pYy_anYCwVvEGLnh88oXAoffSGRLFyHvJjm_KAL6ZfZYLcuXVT_deP_GEhtsfkmyQf_Dp2BprOt5s0GViSQQOhLyQHS-vqOeX95EMG9Dvprw-qi09TouEeu2WVUt9jLyY6PG1ldzZESFZcNt8zggeRQCPy7M29GO6g88HpjLsWgQmm5XQifx51_2bm1MerwO1LvGGnpm4CX5Y0dcoA1c16WKxZgh8ZQkVb6kVUaMo6cD-S-aVwOW2FEqamrU932_seL1V-H7_EyHEGKNKln7mFwWjSoFXKYz8XdrVPk8xHCLbCCXov8ZCM13wlKN9WvooKVlm-hDLDQ_w5so6xXREDzk1KGrU25yUambhRwCfDHX_KBzKDzgw8FCP1ElULjJ4Tsqf5cA35C9omhF8zV3-Q8LFayhxaqttMxcYfjxLNzwvFudI9I-r1ZwYCJ9u8LeZVSnfRCP5idXJL-N4Uw-hXvMS04hDQZxd-ky4q84kvdpU_3pPifvjDq_MvEam_Jew_z0pU1rFBMor5sAzUc1pesiE1E9pchnvkW5B3RHkiHIOSU468tEDebZYy55e5vmaJphJOc1pBcWLjkrktmwnd6b3n3NqnKU6_GDIOKI2A3I3dbLIGLuV_KYDpFwOgfpHBayCSN_r_AXinyRTBkcjTBAT1ymStunQvLHRgfu0kLUYOx38-s3h1XYlAnoXGRllMguS2BoARYHw0rWDQtm77X_CvwqN9CC6Zs3R30UaEG3vt4YjPfLFIXrJSil8XIQiCa71OVLkuCNY-lT7j7Pe"
            };
        }

        static void BuildRequestsConfiguration()
        {
            requestConfigurations = new Dictionary<string, RequestConfiguration>();
            RequestConfiguration tempRequestConfig;

            // localhost admin
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "127.0.0.1";
            tempRequestConfig.HostUri = "http://127.0.0.1:5100";
            tempRequestConfig.PageUri = "";
            tempRequestConfig.HttpMethod = "GET";
            requestConfigurations.Add("a", tempRequestConfig);

            // localhost admin
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "localhost";
            tempRequestConfig.HostUri = "http://localhost:5100";
            tempRequestConfig.PageUri = "authentication/login";
            tempRequestConfig.HttpMethod = "GET";
            requestConfigurations.Add("b", tempRequestConfig);

            // localhost public
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "localhost";
            tempRequestConfig.HostUri = "http://127.0.0.1:5010";
            tempRequestConfig.PageUri = "";
            tempRequestConfig.HttpMethod = "GET";
            requestConfigurations.Add("c", tempRequestConfig);

            // localhost public
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "localhost";
            tempRequestConfig.HostUri = "http://localhost:5010";
            tempRequestConfig.PageUri = "";
            tempRequestConfig.HttpMethod = "GET";
            requestConfigurations.Add("d", tempRequestConfig);

            // staging public
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "staging.g2-dg.com";
            tempRequestConfig.HostUri = "https://staging.g2-dg.com/";
            tempRequestConfig.PageUri = "help/contactcustomerservice"; //"content/about-vital-choice";
            tempRequestConfig.HttpMethod = "POST";
            tempRequestConfig.FormData = new Dictionary<string, string>();
            tempRequestConfig.FormData.Add("Name", "Evgeny");
            tempRequestConfig.FormData.Add("Email", "yauhenlevin@gmail.com");
            tempRequestConfig.FormData.Add("Comment", "test test test test");
            requestConfigurations.Add("e", tempRequestConfig);

            // staging admin
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "admin.staging.g2-dg.com";
            tempRequestConfig.HostUri = "https://admin.staging.g2-dg.com/";
            tempRequestConfig.PageUri = ""; // content/contentpages/115
            tempRequestConfig.HttpMethod = "GET";
            requestConfigurations.Add("f", tempRequestConfig);

            //staging admin
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "admin.staging.g2-dg.com";
            tempRequestConfig.HostUri = "https://admin.staging.g2-dg.com/";
            tempRequestConfig.PageUri = "authentication/login";
            tempRequestConfig.HttpMethod = "GET";
            requestConfigurations.Add("g", tempRequestConfig);

            // staging admin
            tempRequestConfig = new RequestConfiguration();
            tempRequestConfig.Host = "admin.staging.g2-dg.com";
            tempRequestConfig.HostUri = "https://admin.staging.g2-dg.com/";
            tempRequestConfig.PageUri = "Api/Content/GetCategoriesTree"; // content/contentpages/32
            tempRequestConfig.HttpMethod = "POST";
            tempRequestConfig.IsJson = true;
            tempRequestConfig.JsonData = new TestJsonDTO() { Type = "7" };
            requestConfigurations.Add("h", tempRequestConfig);
        }

        static HttpWebRequest ConfigureRequest(RequestConfiguration requestConfig, SessionConfiguration sessionConfig)
        {
            var isPostRequest = requestConfig.HttpMethod == "POST";

            //init uri (with query string)
            var baseUri = new Uri(requestConfig.HostUri);
            var pageUri = new Uri(baseUri, requestConfig.PageUri);
            var request = WebRequest.CreateHttp(pageUri) as HttpWebRequest;

            // init method and headers
            request.Headers = new WebHeaderCollection();
            #region Headers from browser
            //request.KeepAlive = true;
            //request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            
            //AddHeader(request.Headers, "Accept-Language", "en-US,en;q=0.8,ru;q=0.6");
            //AddHeader(request.Headers, "Cache-Control", "max-age=0");
            //AddHeader(request.Headers, "Upgrade-Insecure-Requests", "1");
            // do not enable
            //AddHeader(request.Headers, "Accept-Encoding", "gzip, deflate, sdch"); 
            #endregion

            request.Method = requestConfig.HttpMethod;
            // It is important for admin website requests to contain 'text/html' in Accept header (see HomeRouter.RouteAsync method)
            request.Accept = "text/html,application/json,application/xhtml + xml,application/xml; q = 0.9,image/webp,*/*;q=0.8";

            // init cookies
            request.CookieContainer = new CookieContainer();
            AddCookie(request.CookieContainer, requestConfig, sessionConfig.AdminCookieName, sessionConfig.AdminCookieValue); // TODO: set from request configuration
            #region Cookies from browser
            //AddCookie(request.CookieContainer, "mbdc", "D4D4F694.5FA0.50B8.0999.90A832380F43");
            //AddCookie(request.CookieContainer, "_ga", "GA1.3.918507317.1470743657");
            //AddCookie(request.CookieContainer, "_ceg.s", "obox1o");
            //AddCookie(request.CookieContainer, "_ceg.u", "obox1o");
            //AddCookie(request.CookieContainer, "_ga", "GA1.2.918507317.1470743657");
            //AddCookie(request.CookieContainer, "mbcc", "F38BF22A - DE4B - 5C79 - A498 - 2C45347082C4");
            #endregion

            // init post data 
            if (isPostRequest)
            {
                string dataStrToAppend;

                if (requestConfig.IsJson) // json data
                {
                    request.ContentType = "application/json; charset=UTF-8";
                    dataStrToAppend = JsonConvert.SerializeObject(requestConfig.JsonData);
                }
                else // form data
                {
                    request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    var sb = new StringBuilder();
                    foreach (var item in requestConfig.FormData)
                    {
                        AppendFormData(sb, item.Key, item.Value);
                    }
                    dataStrToAppend = sb.ToString();
                }

                var data = Encoding.UTF8.GetBytes(dataStrToAppend);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }

            return request;
        }

        #region Helper Methods
        
        static string ReadResponse(WebResponse responseObj)
        {
            var response = (HttpWebResponse)responseObj;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                return data;
            }

            return null;
        }

        static void AddHeader(WebHeaderCollection headers, string name, string value)
        {
            headers.Add(name, value);
        }

        static void AddCookie(CookieContainer container, RequestConfiguration requestConfig, string name, string value)
        {
            var cookie = new Cookie(name, value);
            cookie.Domain = requestConfig.Host;
            container.Add(cookie);
        }

        // TODO: just copied from http://stackoverflow.com/questions/3840762/how-do-you-urlencode-without-using-system-web
        static void AppendFormData(StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
                sb.Append("&");
            sb.Append(Uri.EscapeDataString(name));
            sb.Append("=");
            sb.Append(Uri.EscapeDataString(value));
        }

        #endregion

        #region Temp

        #region RequestWithExtendedMetrics

        private static int statusChangedCounter;
        private static void RequestWithExtendedMetrics()
        {
            var client = new WebClient();
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Handler);
            client.DownloadDataAsync(new Uri("https://staging.g2-dg.com/content/about-vital-choice"));
        }

        static void Handler(object sender, DownloadProgressChangedEventArgs args)
        {
            //args.ProgressPercentage;
            statusChangedCounter++;
        }
        #endregion

        private static void TestSuperSimpleRequest()
        {
            WebClient w = new WebClient();
            string s = w.DownloadString("google.by");
        }

        private static void TestSimpleRequest()
        {
            string urlAddress = "https://staging.g2-dg.com/";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }

            Console.ReadKey();
            return;
        }

        #endregion
    }
}
