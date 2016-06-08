namespace VitalChoice.Ecommerce.Domain.Exceptions
{
    public enum MessageType
    {
        FormField = 0,
        FieldAsCode = 1
    }

    public enum MessageLevel
    {
        Error = 0,
        Warning = 1,
        Info = 2
    }

    public struct MessageInfo
    {
        public MessageType MessageType { get; set; }
        public MessageLevel MessageLevel { get; set; }
        public string Field { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{MessageType}:{MessageLevel}]({Field}){Message}";
        }
    }
}