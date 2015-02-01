namespace VitalChoice.Validation.Models
{
    [DataContract]
    public class MessageInfo
    {
        [DataMember]
        public string Field { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}