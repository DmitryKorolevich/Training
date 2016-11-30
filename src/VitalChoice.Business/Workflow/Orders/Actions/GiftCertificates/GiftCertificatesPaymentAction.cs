using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Extensions;
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
            decimal perishableSubtotal = context.SplitInfo.PerishableSubtotal;
            decimal nonPerishableSubtotal = context.SplitInfo.NonPerishableSubtotal;
            // ReSharper disable once PossibleNullReferenceException
            foreach (var gc in context.Order.GiftCertificates)
            {
                if (gc.GiftCertificate.ExpirationDate.HasValue && gc.GiftCertificate.ExpirationDate.Value.AddDays(1) < DateTime.Now)
                {
                    context.GcMessageInfos.Add(new MessageInfo
                    {
                        Message = $"Gift Certificate Expired {gc.GiftCertificate.ExpirationDate.Value:MM'/'dd'/'yyyy}",
                        Field = gc.GiftCertificate.Code
                    });
                }
                if (gc.GiftCertificate.StatusCode != RecordStatusCode.Active)
                {
                    context.GcMessageInfos.Add(new MessageInfo
                    {
                        Message = "Gift Certificate not Active",
                        Field = gc.GiftCertificate.Code
                    });
                    continue;
                }
                var totalGcAmount = gc.GiftCertificate.Balance + gc.Amount;
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
                gc.Amount = context.CombinedStatus != OrderStatus.Incomplete ? charge : 0;
                orderSubTotal -= charge;
                context.GcMessageInfos.Add(new MessageInfo
                {
                    Message = "Successfully used Gift Certificate",
                    Field = gc.GiftCertificate.Code,
                    MessageLevel = MessageLevel.Info
                });

                var perishableCharge = Math.Min(perishableSubtotal, totalGcAmount);
                perishableSubtotal -= perishableCharge;
                var nonPerishableCharge = Math.Min(nonPerishableSubtotal, totalGcAmount - perishableCharge);
                gc.NPAmount = context.CombinedStatus != OrderStatus.Incomplete ? nonPerishableCharge : 0;
                nonPerishableSubtotal -= nonPerishableCharge;
                gc.PAmount = context.CombinedStatus != OrderStatus.Incomplete ? perishableCharge : 0;
            }
            context.GiftCertificatesSubtotal = orderSubTotal - (decimal) context.Data.PayableTotal;
            context.SplitInfo.PerishableGiftCertificateAmount = perishableSubtotal - context.SplitInfo.PerishableSubtotal;
            context.SplitInfo.NonPerishableGiftCertificateAmount = nonPerishableSubtotal - context.SplitInfo.NonPerishableSubtotal;
            return Task.FromResult(context.GiftCertificatesSubtotal);
        }
    }
}