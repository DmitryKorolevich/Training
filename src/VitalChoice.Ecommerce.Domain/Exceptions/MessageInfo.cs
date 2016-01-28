namespace VitalChoice.Ecommerce.Domain.Exceptions
{

    public enum MessageLevel
    {
        Error = 0,
        Warning = 1,
        Info = 2
    }

    public struct MessageInfo
    {
        public MessageLevel MessageLevel { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }
    }
}