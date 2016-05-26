using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Ecommerce.Domain.Options
{
    public class AppOptionsBase
    {
        public Connection Connection { get; set; }
        public string LogPath { get; set; }
        public string LogLevel { get; set; }
        public CacheSettings CacheSettings { get; set; }
        public CacheSyncOptions CacheSyncOptions { get; set; }
    }
}