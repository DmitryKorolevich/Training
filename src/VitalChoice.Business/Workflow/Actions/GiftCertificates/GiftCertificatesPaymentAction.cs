using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Workflow.Actions.GiftCertificates
{
    public class GiftCertificatesPaymentAction : ComputableAction<OrderContext>
    {
        public GiftCertificatesPaymentAction(ComputableTree<OrderContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override decimal ExecuteAction(OrderContext context)
        {
            if (!(context.Order?.GiftCertificates?.Any() ?? false))
            {
                return 0;
            }
            decimal orderSubTotal = context.Data.SubTotal;
            foreach (var gc in context.Order.GiftCertificates)
            {
                var totalGcAmount = gc.Amount + gc.GiftCertificate.Balance;
                if (totalGcAmount == 0)
                {
                    context.Messages.Add(new MessageInfo
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
            return orderSubTotal - (decimal)context.Data.SubTotal;
        }
    }
}