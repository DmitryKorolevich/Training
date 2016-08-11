using System.Runtime.Serialization;

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

    [DataContract]
    public class MessageInfo
    {
        [DataMember]
        public MessageType MessageType { get; set; }
        [DataMember]
        public MessageLevel MessageLevel { get; set; }
        [DataMember]
        public string Field { get; set; }
        [DataMember]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"[{MessageType}:{MessageLevel}]({Field}){Message}";
        }
    }
}