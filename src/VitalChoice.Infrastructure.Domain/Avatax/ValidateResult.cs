namespace VitalChoice.Infrastructure.Domain.Avatax
{
    // Request for address/validate is parsed into the URI query parameters.
    public class ValidateResult
    {
        public Address Address { get; set; }

        public SeverityLevel ResultCode { get; set; }

        public Message[] Messages { get; set; }
    }
}
