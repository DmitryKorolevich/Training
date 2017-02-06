using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ReviewReason
    {
        public OrderReviewRuleDynamic Rule { get; set; }

        public ICollection<string> Reasons { get; set; }
    }
}