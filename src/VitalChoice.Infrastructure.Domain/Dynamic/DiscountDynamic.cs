using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public class DiscountDynamic : MappedObject
    {
        public int? IdAddedBy { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public CustomerType? Assigned { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool ExcludeSkus { get; set; }

        public bool ExcludeCategories { get; set; }

        public ICollection<int> CategoryIds { get; set; }

        public ICollection<DiscountToSku> SkusFilter { get; set; }

        public ICollection<DiscountToSelectedSku> SkusAppliedOnlyTo { get; set; }

        public ICollection<int> CategoryIdsAppliedOnlyTo { get; set; }

        public ICollection<DiscountTier> DiscountTiers { get; set; }
        
        //public string GetDiscountMessage(DiscountTier neededTier)
        //{
        //    string toReturn = String.Empty;
        //    switch (IdObjectType)
        //    {
        //        case (int)DiscountType.FreeShipping:
        //            toReturn = "Free Shipping Discount";
        //            break;
        //        case (int)DiscountType.PercentDiscount:
        //            if (SafeData.Percent != null)
        //            {
        //                toReturn = $"Percent Discount ({(decimal)SafeData.Percent / 100:P0})";
        //            }
        //            break;
        //        case (int)DiscountType.PriceDiscount:
        //            if (SafeData.Amount != null)
        //            {
        //                toReturn = $"Price Discount ({SafeData.Amount:C})";
        //            }
        //            break;
        //        case (int)DiscountType.Threshold:
        //            if (SafeData.ProductSKU != null)
        //            {
        //                toReturn = $"Threshold Discount ({SafeData.ProductSKU})";
        //            }
        //            break;
        //        case (int)DiscountType.Tiered:
        //            if (neededTier != null)
        //            {
        //                switch (neededTier.IdDiscountType)
        //                {
        //                    case DiscountType.PriceDiscount:
        //                        toReturn = $"Tiered Discount, Tier from {neededTier.From:C} to {neededTier.To:C} ({neededTier.Amount ?? 0:C})";
        //                        break;
        //                    case DiscountType.PercentDiscount:
        //                        toReturn = $"Tiered Discount, Tier from {neededTier.From:C} to {neededTier.To:C} ({(neededTier.Percent ?? 0) / 100:P0})";
        //                        break;
        //                }
        //            }
        //            break;
        //    }
        //    return toReturn;
        //}
    }
}