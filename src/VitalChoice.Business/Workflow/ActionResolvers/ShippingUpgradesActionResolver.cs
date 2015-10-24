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

    public class ShippingUpgradesActionResolver : ComputableActionResolver<OrderDataContext>
    {
        public ShippingUpgradesActionResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Order.ShippingAddress == null)
                return Task.FromResult((int) ShippingUpgradeGroup.None);
            if (dataContext.IsCountry(dataContext.Order.ShippingAddress, "us") ||
                dataContext.IsCountry(dataContext.Order.ShippingAddress, "ca"))
            {
                return Task.FromResult((int)ShippingUpgradeGroup.UsCa);
            }
            return Task.FromResult((int) ShippingUpgradeGroup.None);
        }
    }
}
