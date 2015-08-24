using VitalChoice.DynamicData.Entities;
using VitalChoice.Workflow.Base;

namespace VitalChoice.Workflow.Contexts
{
    public class OrderContext: ComputableContext {
        public OrderDynamic Order { get; set; }
    }
}