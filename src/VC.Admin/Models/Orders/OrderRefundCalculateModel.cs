using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Admin.Models.Orders
{
    public class OrderRefundCalculateModel : BaseModel
    {
        public decimal ShippingTotal { get; set; }
        
        public decimal ProductsSubtotal { get; set; }

        public decimal SurchargeOverride { get; set; }

        public decimal ShippingOverride { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }
        
        public decimal Total { get; set; }

        public decimal AutoTotal { get; set; }

        public bool ShippingRefunded { get; set; }

        public decimal ManualShippingTotal { get; set; }

        public decimal RefundGCsUsedOnOrder { get; set; }

        public decimal ManualRefundOverride { get; set; }
        
        public ICollection<RefundSkuManageModel> RefundSkus { get; set; }

        public ICollection<RefundOrderToGiftCertificateManageModel> RefundOrderToGiftCertificates { get; set; }

        public ICollection<MessageInfo> Messages { get; set; }

        public OrderRefundCalculateModel(OrderRefundDataContext dataContext)
        {
            ShippingTotal = dataContext.ShippingTotal;
            ProductsSubtotal = dataContext.ProductsSubtotal;
            DiscountTotal = dataContext.DiscountTotal;
            DiscountedSubtotal = dataContext.DiscountedSubtotal;
            DiscountMessage = dataContext.DiscountMessage;
            TaxTotal = dataContext.TaxTotal;
            Total = dataContext.Total;
            AutoTotal = dataContext.AutoTotal;
            ShippingRefunded = dataContext.ShippingRefunded;
            ManualShippingTotal = dataContext.ManualShippingTotal;
            RefundGCsUsedOnOrder = dataContext.RefundGCsUsedOnOrder;
            ManualRefundOverride = dataContext.ManualRefundOverride;

            RefundSkus = dataContext.RefundSkus?.Select(item => new RefundSkuManageModel(item)).ToList() 
                ?? new List<RefundSkuManageModel>();

            RefundOrderToGiftCertificates = dataContext.RefundOrderToGiftCertificates?.Select(item => new RefundOrderToGiftCertificateManageModel(item)).ToList()
                ?? new List<RefundOrderToGiftCertificateManageModel>();

            Messages = dataContext.Messages;
        }

    }
}