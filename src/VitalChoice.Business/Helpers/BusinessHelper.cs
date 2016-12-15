using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation.Validators;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Orders;

namespace VitalChoice.Business.Helpers
{
    public static class BusinessHelper
    {
        public static string GetDiscountMessage(DiscountDynamic discount, int? IdTier = null, decimal? autoShipDiscountPercent=null)
		{
			string toReturn = null;

		    if (discount == null)
		    {
		        if (autoShipDiscountPercent.HasValue)
		        {
                    toReturn= $"Percent Discount ({autoShipDiscountPercent.Value / 100:P0})";

                }
		        else
                {
                    return null;
                }
		    }

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

        public static string GetDiscountInfo(DiscountDynamic discount, int? IdTier = null, decimal? autoShipDiscountPercent = null)
        {
            string toReturn = null;

            if (discount == null)
            {
                if (autoShipDiscountPercent.HasValue)
                {
                    toReturn = $"{autoShipDiscountPercent.Value / 100:P0}";

                }
                else
                {
                    return null;
                }
            }

            switch (discount.IdObjectType)
            {
                case (int)DiscountType.FreeShipping:
                    toReturn = string.Empty;
                    break;
                case (int)DiscountType.PercentDiscount:
                    if (discount.SafeData.Percent != null)
                    {
                        toReturn = $"{(decimal)discount.SafeData.Percent / 100:P0}";
                    }
                    break;
                case (int)DiscountType.PriceDiscount:
                    if (discount.SafeData.Amount != null)
                    {
                        toReturn = $"{discount.SafeData.Amount:C}";
                    }
                    break;
                case (int)DiscountType.Threshold:
                    if (discount.SafeData.ProductSKU != null)
                    {
                        toReturn = string.Empty;
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
                                        $"{neededTier.Amount ?? 0:C}";
                                    break;
                                case DiscountType.PercentDiscount:
                                    toReturn =
                                        $"{(neededTier.Percent ?? 0) / 100:P0}";
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

        public static Dictionary<string, ImportItemValidationGenericProperty> GetAttrBaseImportValidationSettings(ICollection<PropertyInfo> modelProperties)
        {
            Dictionary<string, ImportItemValidationGenericProperty> toReturn = new Dictionary<string, ImportItemValidationGenericProperty>();
            foreach (var modelProperty in modelProperties)
            {
                var displayAttribute = modelProperty.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault();
                if (displayAttribute != null)
                {
                    ImportItemValidationGenericProperty item = new ImportItemValidationGenericProperty();
                    item.DisplayName = displayAttribute.Name;
                    item.PropertyInfo = modelProperty;
                    item.PropertyType = modelProperty.PropertyType;
                    item.Get = modelProperty.GetMethod?.CompileAccessor<object, object>();
                    var requiredAttribute = modelProperty.GetCustomAttributes<RequiredAttribute>(true).FirstOrDefault();
                    if (requiredAttribute != null)
                    {
                        item.IsRequired = true;
                    }
                    var emailAddressAttribute = modelProperty.GetCustomAttributes<EmailAddressAttribute>(true).FirstOrDefault();
                    if (emailAddressAttribute != null)
                    {
                        item.IsEmail = true;
                    }
                    var maxLengthAttribute = modelProperty.GetCustomAttributes<MaxLengthAttribute>(true).FirstOrDefault();
                    if (maxLengthAttribute != null)
                    {
                        item.MaxLength = maxLengthAttribute.Length;
                    }
                    toReturn.Add(modelProperty.Name, item);
                }
            }

            return toReturn;
        }

        public static void ValidateAttrBaseImportItems(IEnumerable<BaseImportItem> models, Dictionary<string, ImportItemValidationGenericProperty> settings)
        {
            EmailValidator emailValidator = new EmailValidator();
            var emailRegex = new Regex(emailValidator.Expression, RegexOptions.IgnoreCase);
            foreach (var model in models)
            {
                foreach (var pair in settings)
                {
                    var setting = pair.Value;

                    bool valid = true;
                    if (typeof(string) == setting.PropertyType)
                    {
                        string value = (string)setting.Get(model);
                        if (setting.IsRequired && String.IsNullOrEmpty(value))
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired], setting.DisplayName)));
                            valid = false;
                        }

                        if (valid && setting.MaxLength.HasValue && !String.IsNullOrEmpty(value) && value.Length > setting.MaxLength.Value)
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldMaxLength], setting.DisplayName, setting.MaxLength.Value)));
                            valid = false;
                        }

                        if (valid && setting.IsEmail && !String.IsNullOrEmpty(value))
                        {
                            if (!emailRegex.IsMatch(value))
                            {
                                model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsInvalidEmail], setting.DisplayName)));
                            }
                        }
                    }
                }
            }
        }

        public static ICollection<MessageInfo> FormatRowsRecordErrorMessages(IEnumerable<BaseImportItem> items)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            foreach (var item in items)
            {
                toReturn.AddRange(item.ErrorMessages.Select(p => new MessageInfo()
                {
                    Field = p.Field,
                    Message = String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ImportRowError], item.RowNumber, p.Message),
                }));
            }
            return toReturn;
        }

        public static MessageInfo AddErrorMessage(string field, string message)
        {
            return new MessageInfo()
            {
                Field = field ?? "Base",
                Message = message,
            };
        }

        public static bool InStock(this SkuDynamic dynamic)
        {
            return dynamic.IdObjectType == (int)ProductType.EGс || dynamic.IdObjectType == (int)ProductType.Gc ||
                            ((bool?)dynamic.SafeData.DisregardStock ?? false) || ((int?)dynamic.SafeData.Stock ?? 0) > 0;
        }

        public static bool InStock(int idProductType, bool? disregardStock, int? stock)
        {
            return idProductType == (int)ProductType.EGс || idProductType == (int)ProductType.Gc ||
                            (disregardStock ?? false) || (stock ?? 0) > 0;
        }
    }
}
