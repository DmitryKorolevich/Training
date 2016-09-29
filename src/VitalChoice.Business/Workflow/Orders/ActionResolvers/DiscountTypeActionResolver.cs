using System;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.ActionResolvers
{
    public class DiscountTypeActionResolver : ComputableActionResolver<OrderDataContext>
    {
        public DiscountTypeActionResolver(IWorkflowTree<OrderDataContext, decimal> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<int> GetActionKeyAsync(OrderDataContext dataContext, ITreeContext executionContext)
        {
            //Reset discount tier setting
            if (dataContext.Order.DictionaryData.ContainsKey("IdDiscountTier"))
                dataContext.Order.DictionaryData.Remove("IdDiscountTier");

            if (dataContext.Order.Discount == null)
                return 0;
            await ValidateDiscount(dataContext, executionContext);
            return dataContext.Order.Discount.IdObjectType;
        }

        private static async Task ValidateDiscount(OrderDataContext dataContext, ITreeContext executionContext)
        {
            var noIssues = true;

            if (dataContext.Order.Discount.DictionaryData.ContainsKey("RequireMinimumPerishable") &&
                dataContext.Order.Discount.Data.RequireMinimumPerishable &&
                dataContext.Data.PerishableSubtotal < dataContext.Order.Discount.SafeData.RequireMinimumPerishableAmount)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message =
                        $"Minimum perishable {dataContext.Order.Discount.Data.RequireMinimumPerishableAmount:C} not reached",
                    Field = "DiscountCode"
                });
                noIssues = false;
            }
            var now = DateTime.Now;
            if (dataContext.Order.Discount.StatusCode == (int) RecordStatusCode.NotActive)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = "Discount not active",
                    Field = "DiscountCode"
                });
                noIssues = false;
            }
            if (dataContext.Order.Discount.StartDate > now)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = $"Discount not started, start date: {dataContext.Order.Discount.StartDate:d}",
                    Field = "DiscountCode"
                });
                noIssues = false;
            }
            if (dataContext.Order.Discount.ExpirationDate < now)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = $"Discount expired {dataContext.Order.Discount.ExpirationDate:d}",
                    Field = "DiscountCode"
                });
                noIssues = false;
            }
            if (dataContext.Order.Discount.Assigned.HasValue &&
                (int) dataContext.Order.Discount.Assigned.Value != dataContext.Order.Customer.IdObjectType)
            {
                dataContext.Messages.Add(new MessageInfo
                {
                    Message = $"Discount could only be applied for {dataContext.Order.Discount.Assigned.Value} customer",
                    Field = "DiscountCode"
                });
                noIssues = false;
            }

            if (dataContext.Order.Customer?.Id > 0)
            {
                var discountService = executionContext.Resolve<IDiscountService>();
                var usageCount = await discountService.GetDiscountUsed(dataContext.Order.Discount, dataContext.Order.Customer.Id);
                if (dataContext.Order.Id > 0 && dataContext.Order.IsAnyNotIncomplete())
                {
                    var orderRepository = executionContext.Resolve<IEcommerceRepositoryAsync<Order>>();
                    var originalOrder = await orderRepository.Query(o => o.Id == dataContext.Order.Id).SelectFirstOrDefaultAsync(false);
                    if (originalOrder?.IdDiscount != null && originalOrder.IdDiscount.Value == dataContext.Order.Discount.Id)
                    {
                        usageCount--;
                    }
                }
                if (usageCount >= (int?) dataContext.Order.Discount.SafeData.MaxTimesUse)
                {
                    dataContext.Messages.Add(new MessageInfo
                    {
                        Message = "One-time discount code already used",
                        Field = "DiscountCode"
                    });
                    noIssues = false;
                }

                if (noIssues)
                {
                    if ((bool?) dataContext.Order.Discount.SafeData.AllowHealthwise ?? false)
                    {
                        if (dataContext.Order.Data.OrderType == (int) SourceOrderType.Web)
                        {
                            if ((bool?) dataContext.Order.Customer?.SafeData.HasHealthwiseOrders ?? false)
                            {
                                dataContext.Order.IsFirstHealthwise = false;
                            }
                            else if ((bool?) dataContext.Order.SafeData.IsHealthwise ?? false)
                            {
                                dataContext.Order.IsFirstHealthwise = true;
                            }
                        }
                    }
                    else
                    {
                        if (dataContext.Order.Discount.Code.ToLower() != ProductConstants.HEALTHWISE_DISCOUNT_CODE)
                        {
                            dataContext.Order.Data.IsHealthwise = false;
                        }
                        else
                        {
                            if (dataContext.Order.Data.OrderType != (int) SourceOrderType.Web)
                            {
                                dataContext.Messages.Add(new MessageInfo
                                {
                                    Message = "Discount not valid. WEB order only",
                                    Field = "DiscountCode"
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}