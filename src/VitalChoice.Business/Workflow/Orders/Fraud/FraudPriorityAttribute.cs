using System;

namespace VitalChoice.Business.Workflow.Orders.Fraud
{
    public class FraudPriorityAttribute : Attribute
    {
        public int Priority { get; set; }
    }
}