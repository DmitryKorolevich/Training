using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class DiscountFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }

        public string Code { get; set; }
    }
}