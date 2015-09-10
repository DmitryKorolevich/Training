using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class PromotionFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }
    }
}