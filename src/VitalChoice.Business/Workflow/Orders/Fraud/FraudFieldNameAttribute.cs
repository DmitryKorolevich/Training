using System;

namespace VitalChoice.Business.Workflow.Orders.Fraud
{
    public class FraudFieldNameAttribute : Attribute
    {
        public string Name { get; set; }
    }
}