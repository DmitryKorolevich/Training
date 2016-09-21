using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.ActionResolvers
{
    public enum StandardShippingType
    {
        Retail = 1,
        Wholesale = 2,
        Dropship = 3
    }

    public class ShippingStandardResolver : ComputableActionResolver<OrderDataContext>
    {
        public ShippingStandardResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<int> GetActionKeyAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            if (dataContext.FreeShipping)
                return TaskCache<int>.DefaultCompletedTask;
            if (dataContext.Order.ShippingAddress == null)
                return TaskCache<int>.DefaultCompletedTask;
            if (dataContext.Order.IdObjectType == (int) OrderType.DropShip)
            {
                return Task.FromResult((int) StandardShippingType.Dropship);
            }
            var countryNameCode = executionContext.Resolve<ICountryNameCodeResolver>();
            if (dataContext.Order.Customer?.IdObjectType == (int) CustomerType.Wholesale)
            {
                if (countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "us"))
                {
                    return Task.FromResult((int) StandardShippingType.Wholesale);
                }
            }
            else
            {
                if (countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "us") ||
                    countryNameCode.IsCountry(dataContext.Order.ShippingAddress, "ca"))
                {
                    return Task.FromResult((int) StandardShippingType.Retail);
                }
            }

            return TaskCache<int>.DefaultCompletedTask;
        }
    }
}