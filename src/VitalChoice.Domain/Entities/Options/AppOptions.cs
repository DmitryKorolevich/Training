using System;

namespace VitalChoice.Domain.Entities.Options
{
	public class AppOptions
    {
        public bool ServeCdnContent { get; set; }
        public string CdnServerBaseUrl { get; set; }
        public bool GenerateLowercaseUrls { get; set; }
        public bool EnableBundlingAndMinification { get; set; }
        public string RandomPathPart { get; set; }
        public string LogPath { get; set; }
		public int DefaultCacheExpirationTermMinutes { get; set; }
		public int ActivationTokenExpirationTermDays { get; set; }

       public Connection Connection { get; set; }
    }
}
