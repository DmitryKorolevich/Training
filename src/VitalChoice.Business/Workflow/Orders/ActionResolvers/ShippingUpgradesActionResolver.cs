using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.ActionResolvers
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

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            if (dataContext.Order.ShippingAddress == null)
                return Task.FromResult((int) ShippingUpgradeGroup.None);
            var countryNameCode = executionContext.Resolve<ICountryNameCodeResolver>();
            if (countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "us") ||
                countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "ca"))
            {
                return Task.FromResult((int)ShippingUpgradeGroup.UsCa);
            }
            return Task.FromResult((int) ShippingUpgradeGroup.None);
        }
    }
}
