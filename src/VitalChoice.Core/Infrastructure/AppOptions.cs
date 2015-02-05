namespace VitalChoice.Core.Infrastructure
{
	public class AppOptions
	{
		public bool ServeCdnContent { get; set; }
		public string CdnServerBaseUrl { get; set; }
		public bool GenerateLowercaseUrls { get; set; }
		public bool EnableBundlingAndMinification { get; set; }
		public string RandomPathPart { get; set; }
	}
}