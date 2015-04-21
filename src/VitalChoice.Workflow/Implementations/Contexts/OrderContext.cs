using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Implementations.Contexts
{
    public class OrderContext: ComputableContext {
        public ICollection<decimal> ProductCosts { get; set; }
        public double DiscountPercent { get; set; }
    }
}