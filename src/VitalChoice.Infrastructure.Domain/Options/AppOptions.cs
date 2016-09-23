using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class AppOptions : AppOptionsBase
    {
        public string LocalEncryptionKeyPath { get; set; }
        public bool GenerateLowercaseUrls { get; set; }
        public bool EnableBundlingAndMinification { get; set; }
        public int DefaultCacheExpirationTermMinutes { get; set; }
        public int ActivationTokenExpirationTermDays { get; set; }
        public string DefaultCultureId { get; set; }
        public string PublicHost { get; set; }
        public string AdminHost { get; set; }
        public string MainSuperAdminEmail { get; set; }
        public string GiftListUploadEmail { get; set; }
        public string CustomerServiceToEmail { get; set; }
        public string CustomerFeedbackToEmail { get; set; }
        public string OrderShippingNotificationBcc { get; set; }
        public string AffiliateEmailBcc { get; set; }
        public string FilesRelativePath { get; set; }
        public string FilesPath { get; set; }
        public bool EnableOrderTrackScripts { get; set; }
        public EmailConfiguration EmailConfiguration { get; set; }
        public Versioning Versioning { get; set; }
        public AzureStorage AzureStorage { get; set; }
        public FedExOptions FedExOptions { get; set; }
        public AvataxOptions Avatax { get; set; }
        public GoogleCaptcha GoogleCaptcha { get; set; }
        public ExportService ExportService { get; set; }
        public AuthorizeNet AuthorizeNet { get; set; }
        public PDFMyUrl PDFMyUrl { get; set; }
        public BrontoSettings Bronto { get; set; }
        public JobSettings JobSettings { get; set; }
        public VeraCoreSettings VeraCoreSettings { get; set; }
        public GoogleSettings GoogleSettings { get; set; }
        public TwitterSettings TwitterSettings { get; set; }
        public FacebookSettings FacebookSettings { get; set; }
    }
}