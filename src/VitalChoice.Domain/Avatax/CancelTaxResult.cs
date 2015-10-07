using System;

namespace VitalChoice.Domain.Avatax
{
    public class CancelTaxResult
    {
        public SeverityLevel ResultCode { get; set; }

        public string TransactionId { get; set; }

        public string DocId { get; set; }

        public Message[] Messages { get; set; }
    }
}
