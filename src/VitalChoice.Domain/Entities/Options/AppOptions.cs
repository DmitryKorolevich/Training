using System;

namespace VitalChoice.Domain.Entities.Options
{
    public class AppOptions
    {
        public bool GenerateLowercaseUrls { get; set; }
        public bool EnableBundlingAndMinification { get; set; }
        public string LogPath { get; set; }
        public string CustomStylesPath { get; set; }
        public string CustomStylesName { get; set; }
        public int DefaultCacheExpirationTermMinutes { get; set; }
        public int ActivationTokenExpirationTermDays { get; set; }
        public string DefaultCultureId { get; set; }
        public Connection Connection { get; set; }
        public string PublicHost { get; set; }
        public string AdminHost { get; set; }
        public string MainSuperAdminEmail{get;set;}
        public string FilesRelativePath { get; set; }
        public Email EmailConfiguration { get; set; }
		public Versioning Versioning { get; set; }
		public AzureStorage AzureStorage { get; set; }
    }
}
