namespace Authorize.Net.Util
{
#pragma warning disable 1591
    public static class Constants
    {
        public const string ProxyProtocol = "http";

        public const string HttpsUseProxy = "https.proxyUse";
        public const string HttpsProxyHost = "https.proxyHost";
        public const string HttpsProxyPort = "https.proxyPort";

        public const string HttpUseProxy = "http.proxyUse";
        public const string HttpProxyHost = "http.proxyHost";
        public const string HttpProxyPort = "http.proxyPort";

        public const string EnvApiLoginid = "API_LOGIN_ID";
        public const string EnvTransactionKey = "TRANSACTION_KEY";
        public const string EnvMd5Hashkey = "MD5_HASH_KEY";

        public const string PropApiLoginid = "api.login.id";
        public const string PropTransactionKey = "transaction.key";
        public const string PropMd5Hashkey = "md5.hash.key";
    }
#pragma warning restore 1591
}