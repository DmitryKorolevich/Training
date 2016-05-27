namespace VitalChoice.Infrastructure.Domain.Options
{
    public class CacheSyncOptions
    {
        public string ConnectionString { get; set; }
        public string ServiceBusQueueName { get; set; }
        public bool Enabled { get; set; }
    }
}