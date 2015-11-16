using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class AppOptions : AppOptionsBase
    {
        public bool GenerateLowercaseUrls { get; set; }
        public bool EnableBundlingAndMinification { get; set; }
        public int DefaultCacheExpirationTermMinutes { get; set; }
        public int ActivationTokenExpirationTermDays { get; set; }
        public string DefaultCultureId { get; set; }
        public string PublicHost { get; set; }
        public string AdminHost { get; set; }
        public string MainSuperAdminEmail{get;set;}
        public string FilesRelativePath { get; set; }
        public Email EmailConfiguration { get; set; }
		public Versioning Versioning { get; set; }
		public AzureStorage AzureStorage { get; set; }
        public FedExOptions FedExOptions { get; set; }
        public AvataxOptions Avatax { get; set; }
    }
}
