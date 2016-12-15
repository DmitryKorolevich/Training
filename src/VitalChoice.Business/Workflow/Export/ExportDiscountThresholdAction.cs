using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Workflow.Orders.Actions.Discounts;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Export
{
    public class ExportDiscountThresholdAction : DiscountThresholdAction
    {
        public ExportDiscountThresholdAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            dataContext.FreeShipping = dataContext.Order.Discount.Data.FreeShipping;
            dataContext.DiscountMessage = dataContext.Order.Discount.GetDiscountMessage();
            return TaskCache<decimal>.DefaultCompletedTask;
        }
    }
}