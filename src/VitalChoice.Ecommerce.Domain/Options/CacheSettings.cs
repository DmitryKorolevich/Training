namespace VitalChoice.Ecommerce.Domain.Options
{
    public class CacheSettings
    {
        public long MaxProcessHeapsSizeBytes { get; set; }
        public int CacheTimeToLeaveSeconds { get; set; }
        public int CacheScanPeriodSeconds { get; set; }
    }
}