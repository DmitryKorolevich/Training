using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Workflow.Implementations.Contexts
{
    public class OrderContext: WorkflowContext<decimal>
    {
        public ICollection<decimal> ProductCosts { get; set; }
        public double DiscountPercent { get; set; }
    }
}