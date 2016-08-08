namespace VitalChoice.Infrastructure.Domain.Options
{
    public class EmailConfiguration
    {
        public bool Disabled { get; set; }
        public string From { get; set; }
        public string ApiKey { get; set; }
    }
}