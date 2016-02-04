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
        public string MainSuperAdminEmail{get;set; }
        public string CustomerServiceToEmail { get; set; }
        public string CustomerFeedbackToEmail { get; set; }
        public string FilesRelativePath { get; set; }
        public string FilesPath { get; set; }
        public Email EmailConfiguration { get; set; }
		public Versioning Versioning { get; set; }
		public AzureStorage AzureStorage { get; set; }
        public FedExOptions FedExOptions { get; set; }
        public AvataxOptions Avatax { get; set; }
	    public GoogleCaptcha GoogleCaptcha { get; set; }
        public ExportService ExportService { get; set; }
        public CacheSyncOptions CacheSyncOptions { get; set; }
        public AuthorizeNet AuthorizeNet { get; set; }
        public PDFMyUrl PDFMyUrl { get; set; }
    }

    public class CacheSyncOptions
    {
        public string ConnectionString { get; set; }
        public string ServiceBusQueueName { get; set; }
        public bool Enabled { get; set; }
    }
}
