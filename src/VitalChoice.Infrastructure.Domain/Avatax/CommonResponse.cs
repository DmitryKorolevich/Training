namespace VitalChoice.Infrastructure.Domain.Avatax
{
    public enum SeverityLevel
    {
        Success,
        Warning,
        Error,
        Exception
    }
    public class Message // Result object for Common Response Format
    {
        public string Summary { get; set; }

        public string Details { get; set; }

        public string RefersTo { get; set; }

        public SeverityLevel Severity { get; set; }

        public string Source { get; set; }
    }
}
