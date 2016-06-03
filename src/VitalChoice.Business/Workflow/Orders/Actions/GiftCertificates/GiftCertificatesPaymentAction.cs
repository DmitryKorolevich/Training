﻿using System;
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

        public override Task<decimal> ExecuteActionAsync(OrderDataContext dataContext, IWorkflowExecutionContext executionContext)
        {
            if (!((dataContext.Order?.GiftCertificates?.Count ?? 0) > 0))
            {
                return TaskCache<decimal>.DefaultCompletedTask;
            }
            decimal orderSubTotal = dataContext.Data.PayableTotal;
            foreach (var gc in dataContext.Order.GiftCertificates)
            {
                if (gc.GiftCertificate.StatusCode != RecordStatusCode.Active)
                {
                    dataContext.GcMessageInfos.Add(new MessageInfo
                    {
                        Message = "Gift Certificate not Active",
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
            dataContext.GiftCertificatesSubtotal = orderSubTotal - (decimal) dataContext.Data.PayableTotal;
            return Task.FromResult(dataContext.GiftCertificatesSubtotal);
        }
    }
}