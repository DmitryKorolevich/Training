namespace VitalChoice.Validation.Helpers
{
    public static class ValidationPatterns
    {
        public const string EmailPattern ="^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$";
        public const string ContentUrlPattern = @"^[A-Za-z0-9]{1}[A-Za-z0-9-_]{3}[A-Za-z0-9-_]*$";
        public const string RelativeUrlPattern = @"^[A-Za-z0-9-_/?=]*$";
    }
}