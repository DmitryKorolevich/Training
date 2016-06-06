using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Refunds.Actions
{
    public class RefundGiftCertificatesAction: ComputableAction<OrderRefundDataContext>
    {
        public RefundGiftCertificatesAction(ComputableTree<OrderRefundDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, ITreeContext executionContext)
        {
            decimal total = Math.Min((decimal) context.Order.Data.RefundGCsUsedOnOrder, context.AutoTotal);
            decimal maxRefunded = 0;
            foreach (var gc in context.Order.RefundOrderToGiftCertificates)
            {
                var possibleToRefund = gc.AmountUsedOnSourceOrder - gc.AmountRefunded;
                if (possibleToRefund < 0)
                {
                    throw new ApiException("Oops something went wrong with Gift Certificates refund");
                }
                var toRefundNew = Math.Min(total, possibleToRefund);
                gc.Amount = toRefundNew;
                total -= toRefundNew;
                maxRefunded += toRefundNew;
            }
            context.RefundGCsUsedOnOrder = maxRefunded;
            context.RefundOrderToGiftCertificates = context.Order.RefundOrderToGiftCertificates;
            return Task.FromResult(-maxRefunded);
        }
    }
}
