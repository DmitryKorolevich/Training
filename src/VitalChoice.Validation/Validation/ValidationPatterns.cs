namespace VitalChoice.Validation.Validation
{
    public static class ValidationPatterns {
        //public const string EmailPattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})$";
        public const string DomainPattern = @"^[A-Za-z0-9]{1}[A-Za-z0-9_-]*$";
        public const string SiteMapVerificationTag = "^\\s*<meta[^>]*name\\s*=\\s*\"google-site-verification\"[^>]*/>\\s*$";
        public const string YouTubeURLValidPatter = @"^((http|https)://){0,1}(www.){0,1}youtube.com(/[\w-_,./?%:/+&=#!]*)?$";
        public const string YouTubeShortURLValidPatter = @"^((http|https)://){0,1}(www.){0,1}youtu.be(/[\w-_,./?%:/+&=#!]*)?$";
    }
}