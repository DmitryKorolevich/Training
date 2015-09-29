using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Shipping;
using VitalChoice.Workflow.Contexts;

namespace VC.Admin.Models.Order
{
    public class OrderCalculateModel : BaseModel
    {
        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public IList<LookupItem<int?>> ShippingUpgradePOptions { get; set; }

        public IList<LookupItem<int?>> ShippingUpgradeNPOptions { get; set; }

        public int? ShippingUpgradeP { get; set; }

        public int? ShippingUpgradeNP { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool ProductsPerishableThresholdIssue { get; set; }

        public IList<SkuOrderedManageModel> SkuOrdereds { get; set; }

        public IList<PromoSkuOrderedManageModel> PromoSkus { get; set; }

        public IList<MessageInfo> Messages { get; set; }

        public OrderCalculateModel(OrderContext context)
        {
            AlaskaHawaiiSurcharge = context.AlaskaHawaiiSurcharge;
            CanadaSurcharge = context.CanadaSurcharge;
            StandardShippingCharges = context.StandardShippingCharges;
            ShippingUpgradePOptions = context.ShippingUpgradePOptions;
            ShippingUpgradeNPOptions = context.ShippingUpgradeNpOptions;
            ShippingTotal = context.ShippingTotal;
            ProductsSubtotal = context.ProductsSubtotal;
            DiscountTotal = context.DiscountTotal;
            DiscountedSubtotal = context.DiscountedSubtotal;
            DiscountMessage = context.DiscountMessage;
            TaxTotal = context.TaxTotal;
            Total = context.Total;

            SkuOrdereds = context.SkuOrdereds?.Select(item => new SkuOrderedManageModel(item)).ToList() ?? new List<SkuOrderedManageModel>();

            PromoSkus = context.PromoSkus?.Select(item => new PromoSkuOrderedManageModel(item)).ToList() ?? new List<PromoSkuOrderedManageModel>();

            Messages = context.Messages;
        }

    }
}