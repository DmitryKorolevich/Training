namespace Authorize.Net
{
    /*================================================================================
    * 
    * Determines the target environment to post transactions.
    *
    * SANDBOX should be used for testing. Transactions submitted to the sandbox 
    * will not result in an actual card payment. Instead, the sandbox simulates 
    * the response. Use the Testing Guide to generate specific gateway responses.
    *
    * PRODUCTION connects to the production gateway environment.
    *
    *===============================================================================*/

    public class Environment
    {
        public static readonly Environment Sandbox = new Environment("https://test.authorize.net", "https://apitest.authorize.net",
            "https://test.authorize.net");

        public static readonly Environment Production = new Environment("https://secure2.authorize.net", "https://api2.authorize.net",
            "https://cardpresent.authorize.net");

        public static readonly Environment LocalVm = new Environment(null, null, null);
        public static readonly Environment HostedVm = new Environment(null, null, null);
        public static readonly Environment Custom = new Environment(null, null, null);

        private string _baseUrl;
        private string _cardPresentUrl;
        private string _xmlBaseUrl;

        private Environment(string baseUrl, string xmlBaseUrl, string cardPresentUrl)
        {
            _baseUrl = baseUrl;
            _xmlBaseUrl = xmlBaseUrl;
            _cardPresentUrl = cardPresentUrl;
        }

        /**
	     * @return the baseUrl
	     */

        public string GetBaseUrl()
        {
            return _baseUrl;
        }

        /**
	     * @return the xmlBaseUrl
	     */

        public string GetXmlBaseUrl()
        {
            return _xmlBaseUrl;
        }

        /**
	     * @return the cardPresentUrl
	     */

        public string GetCardPresentUrl()
        {
            return _cardPresentUrl;
        }

        /**
	     * If a custom environment needs to be supported, this convenience create
	     * method can be used to pass in a custom baseUrl.
	     *
	     * @param baseUrl
	     * @param xmlBaseUrl
	     * @return Environment object
	     */

        public static Environment CreateEnvironment(string baseUrl, string xmlBaseUrl)
        {
            return CreateEnvironment(baseUrl, xmlBaseUrl, null);
        }

        /**
	     * If a custom environment needs to be supported, this convenience create
	     * method can be used to pass in a custom baseUrl.
	     *
	     * @param baseUrl
	     * @param xmlBaseUrl
	     * @param cardPresentUrl
	     *
	     * @return Environment object
	     */

        public static Environment CreateEnvironment(string baseUrl, string xmlBaseUrl, string cardPresentUrl)
        {
            var environment = Custom;
            environment._baseUrl = baseUrl;
            environment._xmlBaseUrl = xmlBaseUrl;
            environment._cardPresentUrl = cardPresentUrl;

            return environment;
        }
    }
}