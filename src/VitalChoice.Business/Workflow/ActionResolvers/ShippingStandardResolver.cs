using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.ActionResolvers
{
    public class ShippingStandardResolver : ComputableActionResolver<OrderDataContext>
    {
        public ShippingStandardResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (dataContext.FreeShipping)
                return Task.FromResult(0);
            if (dataContext.Order.ShippingAddress == null)
                return Task.FromResult(0);
            var countryNameCode = executionContext.Resolve<ICountryNameCodeResolver>();
            if (dataContext.Order.Customer.IdObjectType == (int) CustomerType.Wholesale)
            {
                if (countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "us"))
                {
                    return Task.FromResult((int) CustomerType.Wholesale);
                }
            }
            if (dataContext.Order.Customer.IdObjectType == (int) CustomerType.Retail)
            {
                if (countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "us") ||
                    countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "ca"))
                {
                    return Task.FromResult((int) CustomerType.Retail);
                }
            }

            return Task.FromResult(0);
        }
    }
}