using System.Collections.Generic;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Business.Workflow.Contexts
{
    public class OrderContext: ComputableContext {
        public ICollection<decimal> ProductCosts { get; set; }
        public double DiscountPercent { get; set; }
    }
}