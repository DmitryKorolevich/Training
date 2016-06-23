using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions.GiftCertificates
{
    public class GiftCertificatesPaymentAction : ComputableAction<OrderDataContext>
    {
        public GiftCertificatesPaymentAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            if ((context.Order.GiftCertificates?.Count ?? 0) == 0)
            {
                return TaskCache<decimal>.DefaultCompletedTask;
            }
            decimal orderSubTotal = context.Data.PayableTotal;
            // ReSharper disable once PossibleNullReferenceException
            foreach (var gc in context.Order.GiftCertificates)
            {
                if (gc.GiftCertificate.StatusCode != RecordStatusCode.Active)
                {
                    context.GcMessageInfos.Add(new MessageInfo
                    {
                        Message = "Gift Certificate not Active",
                        Field = gc.GiftCertificate.Code
                    });
                    continue;
                }
                var totalGcAmount = gc.Amount + gc.GiftCertificate.Balance;
                if (totalGcAmount == 0)
                {
                    context.GcMessageInfos.Add(new MessageInfo
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
                context.GcMessageInfos.Add(new MessageInfo
                {
                    Message = "Successfully used Gift Certificate",
                    Field = gc.GiftCertificate.Code,
                    MessageLevel = MessageLevel.Info
                });
            }
            context.GiftCertificatesSubtotal = orderSubTotal - (decimal) context.Data.PayableTotal;
            return Task.FromResult(context.GiftCertificatesSubtotal);
        }
    }
}