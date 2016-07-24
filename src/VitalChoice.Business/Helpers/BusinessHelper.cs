using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Helpers
{
    public static class BusinessHelper
    {
		public static string GetDiscountMessage(this DiscountDynamic discount, int? IdTier = null)
		{
			string toReturn = null;
			if (discount == null)
				return null;
			switch (discount.IdObjectType)
			{
				case (int)DiscountType.FreeShipping:
					toReturn = "Free Shipping Discount";
					break;
				case (int)DiscountType.PercentDiscount:
					if (discount.SafeData.Percent != null)
					{
						toReturn = $"Percent Discount ({(decimal)discount.SafeData.Percent / 100:P0})";
					}
					break;
				case (int)DiscountType.PriceDiscount:
					if (discount.SafeData.Amount != null)
					{
						toReturn = $"Price Discount ({discount.SafeData.Amount:C})";
					}
					break;
				case (int)DiscountType.Threshold:
					if (discount.SafeData.ProductSKU != null)
					{
						toReturn = $"Threshold Discount ({discount.SafeData.ProductSKU})";
					}
					break;
				case (int)DiscountType.Tiered:
					if (IdTier.HasValue)
					{
						var neededTier = discount.DiscountTiers?.FirstOrDefault(p => p.Id == IdTier.Value);
						if (neededTier != null)
						{
							switch (neededTier.IdDiscountType)
							{
								case DiscountType.PriceDiscount:
									toReturn =
										$"Tiered Discount, Tier from {neededTier.From:C} to {neededTier.To:C} ({neededTier.Amount ?? 0:C})";
									break;
								case DiscountType.PercentDiscount:
									toReturn =
										$"Tiered Discount, Tier from {neededTier.From:C} to {neededTier.To:C} ({(neededTier.Percent ?? 0) / 100:P0})";
									break;
							}
						}
					}
					else
					{
					    toReturn = "Tiered Discount";
					}
					break;
			}
			return toReturn;
		}
	}
}
