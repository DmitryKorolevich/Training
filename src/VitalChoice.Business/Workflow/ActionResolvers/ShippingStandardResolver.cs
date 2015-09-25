using System.Linq;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Helpers;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
    public class ShippingStandardResolver : ComputableActionResolver<OrderContext>
    {
        public ShippingStandardResolver(IWorkflowTree<OrderContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override int GetActionKey(OrderContext context)
        {
            if (context.FreeShipping)
                return 0;
            if (context.Order.ShippingAddress == null)
                return 0;
            if (context.Order.Customer.IdObjectType == (int) CustomerType.Wholesale)
            {
                if (context.Order.ShippingAddress.IdCountry == context.Coutries.GetCountryId("us"))
                {
                    return (int) CustomerType.Wholesale;
                }
            }
            if (context.Order.Customer.IdObjectType == (int) CustomerType.Retail)
            {
                if (context.Order.ShippingAddress.IdCountry ==
                    context.Coutries.GetCountryId("ca") ||
                    context.Order.ShippingAddress.IdCountry ==
                    context.Coutries.GetCountryId("us"))
                {
                    return (int) CustomerType.Retail;
                }
            }

            return 0;
        }
    }
}