﻿using System.Collections.Generic;
using System.Linq;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Admin.Models.Orders
{
    public class OrderCalculateModel : BaseModel
    {
        public decimal AlaskaHawaiiSurcharge { get; set; }

        public decimal CanadaSurcharge { get; set; }

        public decimal StandardShippingCharges { get; set; }

        public IList<LookupItem<ShippingUpgradeOption>> ShippingUpgradePOptions { get; set; }

        public IList<LookupItem<ShippingUpgradeOption>> ShippingUpgradeNPOptions { get; set; }

        public ShippingUpgradeOption? ShippingUpgradeP { get; set; }

        public ShippingUpgradeOption? ShippingUpgradeNP { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TotalShipping { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal SurchargeOverride { get; set; }

        public decimal ShippingOverride { get; set; }

        public decimal DiscountTotal { get; set; }

        public decimal DiscountedSubtotal { get; set; }

        public string DiscountMessage { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public bool ProductsPerishableThresholdIssue { get; set; }

        public bool ShouldSplit { get; set; }

        public IList<SkuOrderedManageModel> SkuOrdereds { get; set; }

        public IList<PromoSkuOrderedManageModel> PromoSkus { get; set; }

        public IList<MessageInfo> Messages { get; set; }

        public OrderCalculateModel(OrderDataContext dataContext)
        {
            AlaskaHawaiiSurcharge = dataContext.AlaskaHawaiiSurcharge;
            CanadaSurcharge = dataContext.CanadaSurcharge;
            StandardShippingCharges = dataContext.StandardShippingCharges;
            ShippingUpgradePOptions = dataContext.ShippingUpgradePOptions;
            ShippingUpgradeNPOptions = dataContext.ShippingUpgradeNpOptions;
            ShippingTotal = dataContext.ShippingTotal;
            ProductsSubtotal = dataContext.ProductsSubtotal;
            DiscountTotal = dataContext.DiscountTotal;
            DiscountedSubtotal = dataContext.DiscountedSubtotal;
            DiscountMessage = dataContext.DiscountMessage;
            TaxTotal = dataContext.TaxTotal;
            Total = dataContext.Total;
            ShippingOverride = dataContext.ShippingOverride;
            SurchargeOverride = dataContext.SurchargeOverride;
            TotalShipping = dataContext.TotalShipping;
            ShouldSplit = dataContext.SplitInfo.ShouldSplit;
            ProductsPerishableThresholdIssue = dataContext.ProductsPerishableThresholdIssue;

            SkuOrdereds = dataContext.SkuOrdereds?.Select(item => new SkuOrderedManageModel(item)).ToList() ?? new List<SkuOrderedManageModel>();

            PromoSkus = dataContext.PromoSkus?.Select(item => new PromoSkuOrderedManageModel(item)).ToList() ?? new List<PromoSkuOrderedManageModel>();

            Messages = dataContext.Messages;
        }

    }
}