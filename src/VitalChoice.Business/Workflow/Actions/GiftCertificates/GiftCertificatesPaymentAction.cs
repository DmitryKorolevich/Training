using System;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions.GiftCertificates
{
    public class GiftCertificatesPaymentAction : ComputableAction<OrderDataContext>
    {
        public GiftCertificatesPaymentAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (!(dataContext.Order?.GiftCertificates?.Any() ?? false))
            {
                return Task.FromResult<decimal>(0);
            }
            decimal orderSubTotal = dataContext.Data.SubTotal;
            foreach (var gc in dataContext.Order.GiftCertificates)
            {
                var totalGcAmount = gc.Amount + gc.GiftCertificate.Balance;
                if (totalGcAmount == 0)
                {
                    dataContext.Messages.Add(new MessageInfo
                    {
                        Message = "Zero balance Gift Certificate",
                        Field = "GiftCertificateCode"
                    });
                }
                var charge = Math.Min(orderSubTotal, totalGcAmount);
                gc.GiftCertificate.Balance += gc.Amount - charge;
                gc.Amount = charge;
                orderSubTotal -= charge;
            }
            return Task.FromResult(orderSubTotal - (decimal) dataContext.Data.SubTotal);
        }
    }
}