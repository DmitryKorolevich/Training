using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.Shipping
{
    public class StandardShippingUsWholesaleAction : ComputableAction<OrderContext>
    {
        public StandardShippingUsWholesaleAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            if (context.Data.PromoProducts < 200 && context.Data.DeliveredAmount > 0)
            {
                if (context.Data.PromoProducts < 50)
                {
                    context.StandardShippingCharges = (decimal) 4.95;
                    return context.StandardShippingCharges;
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.FirstCost;
                }
                if (context.Data.PromoProducts < 100)
                {
                    context.StandardShippingCharges = (decimal) 9.95;
                    return context.StandardShippingCharges;
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
                if (context.Data.PromoProducts < 150)
                {
                    context.StandardShippingCharges = (decimal)14.95;
                    return context.StandardShippingCharges;
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
                if (context.Data.PromoProducts < 200)
                {
                    context.StandardShippingCharges = (decimal)19.95;
                    return context.StandardShippingCharges;
                    //_deliveryServiceCostGroup = DeliveryServiceCostGroup.SecondCost;
                }
            }
            return 0;
        }
    }
}