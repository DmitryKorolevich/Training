using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VC.Admin.Validators.Order;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Products;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Admin.Models.Orders
{
    public class RefundSkuManageModel : BaseModel
    {
        public int? Id { get; set; }

        public int? IdSku { get; set; }

        public string Code { get; set; }

        public int? IdProductType { get; set; }

        public string ProductName { get; set; }

        public RedeemType Redeem { get; set; }

        public int Quantity { get; set; }

        public decimal RefundPrice { get; set; }

        public decimal RefundValue { get; set; }

        public double RefundPercent { get; set; }

        public IList<MessageInfo> Messages { get; set; }

        public bool Disabled { get; set; }

        public bool Active { get; set; }

        [JsonConstructor]
        public RefundSkuManageModel(RefundSkuOrdered model)
        {
            if (model != null)
            {
                if (model.Sku?.Product != null)
                {
                    ProductName = SkuOrderedManageModel.FormatProductName(model.Sku);
                    IdProductType = model.Sku.IdObjectType;
                }
                Quantity = model.Quantity;
                Redeem = model.Redeem;
                RefundPrice = model.RefundPrice;
                RefundPercent = model.RefundPercent;
                RefundValue = model.RefundValue;

                if (model.Sku != null)
                {
                    Id = model.Sku.Id;
                    IdSku= model.Sku.Id;
                    Code = model.Sku.Code;
                }

                Messages = model.Messages;
            }
        }

        public RefundSkuManageModel(SkuOrdered model)
        {
            if (model != null)
            {
                if (model.Sku?.Product != null)
                {
                    ProductName = SkuOrderedManageModel.FormatProductName(model.Sku);
                    IdProductType = model.Sku.IdObjectType;
                }
                Quantity = model.Quantity;
                Redeem = RedeemType.Refund;
                RefundPrice = model.Amount;
                RefundPercent = 100;
                RefundValue = Quantity*model.Amount;

                if (model.Sku != null)
                {
                    Id = model.Sku.Id;
                    IdSku = model.Sku.Id;
                    Code = model.Sku.Code;
                }

                Messages = model.Messages;
            }
        }
    }

    public class RefundOrderToGiftCertificateManageModel : BaseModel
    {
        public int IdOrder { get; set; }

        public int IdGiftCertificate { get; set; }

        public decimal AmountUsedOnSourceOrder { get; set; }

        public decimal AmountRefunded { get; set; }

        public decimal Amount { get; set; }

        public string Code { get; set; }

        public IList<string> Messages { get; set; }

        [JsonConstructor]
        public RefundOrderToGiftCertificateManageModel(RefundOrderToGiftCertificateUsed model)
        {
            if (model != null)
            {
                IdOrder = model.IdOrder;
                IdGiftCertificate = model.IdGiftCertificate;
                AmountUsedOnSourceOrder = model.AmountUsedOnSourceOrder;
                AmountRefunded = model.AmountRefunded;
                Amount = model.Amount;
                Code = model.Code;

                Messages = model.Messages;
            }
        }

        public RefundOrderToGiftCertificateManageModel(GiftCertificateInOrder model, int idOrder)
        {
            if (model != null)
            {
                IdOrder = idOrder;
                IdGiftCertificate = model.GiftCertificate.Id;
                AmountUsedOnSourceOrder = model.Amount;
                Amount = 0;
                Code = model.GiftCertificate.Code;

                Messages = new List<string>();
            }
        }
    }


    [ApiValidator(typeof(OrderRefundManageModelValidator))]
    public class OrderRefundManageModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public int IdCustomer { get; set; }

        [Map]
        public DateTime DateCreated { get; set; }

        [DirectLocalized("Shipping")]
        public AddressModel Shipping { get; set; }

        [DirectLocalized("On Approved Credit")]
        public OacRefundPaymentModel Oac { get; set; }

        [DirectLocalized("Payment Method")]
        public int? IdPaymentMethodType { get; set; }

        [Map]
        public int? IdObjectType { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public OrderStatus? OrderStatus { get; set; }

        [Map]
        public string OrderNotes { get; set; }

        [Map]
        public decimal ShippingTotal { get; set; }
        
        [Map]
        public decimal ProductsSubtotal { get; set; }

        public string DiscountCode { get; set; }
        
        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        [Map]
        public decimal DiscountTotal { get; set; }

        [Map]
        public decimal TaxTotal { get; set; }

        [Map]
        public decimal AutoTotal { get; set; }

        [Map]
        public decimal Total { get; set; }

        [Map]
        public IList<RefundSkuManageModel> RefundSkus { get; set; }

        [Map]
        public IList<RefundOrderToGiftCertificateManageModel> RefundOrderToGiftCertificates { get; set; }
        
        [Map]
        public int? IdOrderSource { get; set; }

        [Map]
        public int? ServiceCode { get; set; }

        [Map]
        public bool ReturnAssociated { get; set; }

        public bool DisableShippingRefunded { get; set; }

        [Map]
        public bool ShippingRefunded { get; set; }

        [Map]
        public decimal ManualShippingTotal { get; set; }

        [Map]
        public decimal ManualRefundOverride { get; set; }

        [Map]
        public decimal RefundGCsUsedOnOrder { get; set; }

        public decimal OrderSourceTotal { get; set; }

        public DateTime OrderSourceDateCreated { get; set; }

        public int? OrderSourcePaymentMethodType { get; set; }
        
        public ICollection<int> OrderSourceRefundIds { get; set; }

        public decimal GiftCertificatesUsedAmountOnSourceOrder { get; set; }
        
        public OrderRefundManageModel()
        {
            RefundSkus = new List<RefundSkuManageModel>();
        }

    }
}