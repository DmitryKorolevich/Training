using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.ActionResolvers
{
    public class RefundDiscountTypeActionResolver : ComputableActionResolver<OrderRefundDataContext>
    {
        public RefundDiscountTypeActionResolver(IWorkflowTree<OrderRefundDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<int> GetActionKeyAsync(OrderRefundDataContext dataContext, ITreeContext executionContext)
        {
            //Reset discount tier setting
            if (dataContext.RefundOrder.DictionaryData.ContainsKey("IdDiscountTier"))
                dataContext.RefundOrder.DictionaryData.Remove("IdDiscountTier");

            if (dataContext.RefundOrder.Discount == null)
                return TaskCache<int>.DefaultCompletedTask;
            return Task.FromResult(dataContext.RefundOrder.Discount.IdObjectType);
        }
    }
}