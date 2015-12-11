namespace VitalChoice.Ecommerce.Domain.Options
{
    public class AppOptionsBase
    {
        public Connection Connection { get; set; }
        public string LogPath { get; set; }
        public string LogLevel { get; set; }
    }
}