using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
    public enum SurchargeType
    {
        None = 0,
        AlaskaHawaii = 1,
        Canada = 2
    }

    public class ShippingSurchargeResolver : ComputableActionResolver<OrderDataContext>
    {
        public ShippingSurchargeResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName)
            : base(tree, actionName)
        {
        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.Order.ShippingAddress == null)
                return Task.FromResult((int) SurchargeType.None);
            var countryNameCode = executionContext.Resolve<ICountryNameCodeResolver>();
            if (countryNameCode.IsState(dataContext.Order.ShippingAddress, "us", "hi") ||
                countryNameCode.IsState(dataContext.Order.ShippingAddress, "us", "ak"))
            {
                return Task.FromResult((int) SurchargeType.AlaskaHawaii);
            }
            if (countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "ca"))
            {
                return Task.FromResult((int) SurchargeType.Canada);
            }
            return Task.FromResult((int) SurchargeType.None);
        }
    }
}