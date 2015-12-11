namespace VitalChoice.Infrastructure.Domain.Options
{
    public class AuthorizeNet
    {
        public string ApiKey { get; set; }
        public string ApiLogin { get; set; }
        public bool TestEnv { get; set; }
    }
}