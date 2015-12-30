using System;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class PromotionFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateStatus? DateStatus { get; set; }
    }
}