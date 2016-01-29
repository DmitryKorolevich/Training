using System;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
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
                if (gc.GiftCertificate.StatusCode != RecordStatusCode.Active)
                {
                    dataContext.GcMessageInfos.Add(new MessageInfo
                    {
                        Message = "Gift Certificate Not Active",
                        Field = gc.GiftCertificate.Code
                    });
                    continue;
                }
                var totalGcAmount = gc.Amount + gc.GiftCertificate.Balance;
                if (totalGcAmount == 0)
                {
                    dataContext.GcMessageInfos.Add(new MessageInfo
                    {
                        Message = "Zero balance Gift Certificate",
                        Field = gc.GiftCertificate.Code
                    });
                    continue;
                }
                var charge = Math.Min(orderSubTotal, totalGcAmount);
                gc.GiftCertificate.Balance += gc.Amount - charge;
                gc.Amount = charge;
                orderSubTotal -= charge;
                dataContext.GcMessageInfos.Add(new MessageInfo
                {
                    Message = "Successfully used Gift Certificate",
                    Field = gc.GiftCertificate.Code,
                    MessageLevel = MessageLevel.Info
                });
            }
            dataContext.GiftCertificatesSubtotal = orderSubTotal - (decimal) dataContext.Data.SubTotal;
            return Task.FromResult(dataContext.GiftCertificatesSubtotal);
        }
    }
}