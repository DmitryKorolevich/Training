using System;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class DiscountFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }

        public string Code { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}