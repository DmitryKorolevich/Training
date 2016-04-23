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

        public override Task<decimal> ExecuteActionAsync(OrderRefundDataContext context, IWorkflowExecutionContext executionContext)
        {
            context.RefundOrderToGiftCertificates = new List<RefundOrderToGiftCertificateUsed>();
            decimal total = Math.Min((decimal) context.Order.Data.RefundGCsUsedOnOrder,
                (decimal) context.Data.RefundSubtotal);
            decimal maxRefunded = 0;
            foreach (var gc in context.Order.RefundOrderToGiftCertificates)
            {
                if (total > 0)
                {
                    var possibleToRefund = gc.AmountUsedOnSourceOrder - gc.AmountRefunded;
                    if (possibleToRefund < 0)
                    {
                        throw new ApiException("Opps something went wrong with Gift Certificates refund");
                    }
                    var toRefundNew = Math.Min(total, possibleToRefund);
                    var newGc = new RefundOrderToGiftCertificateUsed
                    {
                        Amount = gc.Amount + toRefundNew,
                        AmountRefunded = gc.AmountRefunded + toRefundNew,
                        AmountUsedOnSourceOrder = gc.AmountUsedOnSourceOrder,
                        Code = gc.Code,
                        IdGiftCertificate = gc.IdGiftCertificate,
                        IdOrder = gc.IdOrder,
                        Messages = new List<string>()
                    };
                    context.RefundOrderToGiftCertificates.Add(newGc);
                    total -= toRefundNew;
                    maxRefunded += toRefundNew;
                }
            }
            context.RefundGCsUsedOnOrder = maxRefunded;
            return Task.FromResult(-maxRefunded);
        }
    }
}
