using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Helpers;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
    public enum ShippingUpgradeGroup
    {
        None = 0,
        UsCa = 1
    }

    public class ShippingUpgradesActionResolver : ComputableActionResolver<OrderContext>
    {
        public ShippingUpgradesActionResolver(IWorkflowTree<OrderContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override int GetActionKey(OrderContext context)
        {
            if (context.Order.ShippingAddress == null)
                return (int) ShippingUpgradeGroup.None;
            if (context.IsCountry(context.Order.ShippingAddress, "us") ||
                context.IsCountry(context.Order.ShippingAddress, "ca"))
            {
                return (int)ShippingUpgradeGroup.UsCa;
            }
            return (int) ShippingUpgradeGroup.None;
        }
    }
}
