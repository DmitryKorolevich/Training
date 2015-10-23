using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
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

            SkuOrdereds = dataContext.SkuOrdereds?.Select(item => new SkuOrderedManageModel(item)).ToList() ?? new List<SkuOrderedManageModel>();

            PromoSkus = dataContext.PromoSkus?.Select(item => new PromoSkuOrderedManageModel(item)).ToList() ?? new List<PromoSkuOrderedManageModel>();

            Messages = dataContext.Messages;
        }

    }
}