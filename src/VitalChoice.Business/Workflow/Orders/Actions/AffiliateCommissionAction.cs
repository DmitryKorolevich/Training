using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions
{
    public class AffiliateCommissionAction : ComputableAction<OrderDataContext>
    {
        public AffiliateCommissionAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context, IWorkflowExecutionContext executionContext)
        {
            if (context.Order.Customer?.IdAffiliate != null && context.Order.Customer.IdAffiliate.Value > 0 &&
                context.Order.Customer?.Id > 0)
            {
                var customerService = executionContext.Resolve<ICustomerService>();
                var affiliateService = executionContext.Resolve<IAffiliateService>();
                var hasOrders =
                    await customerService.GetCustomerHasAffiliateOrders(context.Order.Customer.Id, context.Order.Id);
                var affiliate = await affiliateService.SelectAsync(context.Order.Customer.IdAffiliate.Value, true);
                if (affiliate != null)
                {
                    decimal result;

                    decimal baseAmount = (decimal) context.Data.PromoProducts;
                    object discount;
                    if (context.DictionaryData.TryGetValue("Discount", out discount))
                    {
                        baseAmount += (decimal) discount;
                    }
                    if (hasOrders)
                    {
                        context.Order.AffiliateNewCustomerOrder = false;
                        result = baseAmount*affiliate.CommissionAll/100;
                    }
                    else
                    {
                        context.Order.AffiliateNewCustomerOrder = true;
                        result = baseAmount*affiliate.CommissionFirst/100;
                    }
                    context.Order.AffiliatePaymentAmount = Math.Round(result, 2);
                    return result;
                }
            }
            return 0;
        }
    }
}