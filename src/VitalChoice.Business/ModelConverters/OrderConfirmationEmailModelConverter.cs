using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Options;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.Business.ModelConverters
{
    public class OrderConfirmationEmailModelConverter : BaseModelConverter<OrderConfirmationEmail, OrderDynamic>
    {
        private readonly ICustomerService _customerService;
        private readonly ReferenceData _referenceData;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IOptions<AppOptions> _options;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public OrderConfirmationEmailModelConverter(
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICustomerService customerService,
            IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<SkuDynamic, Sku> skuMapper,
            IDynamicMapper<ProductDynamic, Product> productMapper,
            IOptions<AppOptions> options, ICountryNameCodeResolver countryNameCodeResolver)
        {
            _customerService = customerService;
            _referenceData = appInfrastructureService.Data();
            _addressMapper = addressMapper;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _options = options;
            _countryNameCodeResolver = countryNameCodeResolver;
        }

        public override async Task DynamicToModelAsync(OrderConfirmationEmail model, OrderDynamic dynamic)
        {
            dynamic.Customer = await _customerService.SelectAsync(dynamic.Customer.Id);
            model.PublicHost = _options.Value.PublicHost;

            //Dates in the needed timezone
            model.DateCreated = TimeZoneInfo.ConvertTime(model.DateCreated, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
            if (model.ShipDelayDate.HasValue)
            {
                model.ShipDelayDate = TimeZoneInfo.ConvertTime(model.ShipDelayDate.Value, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
            }
            if (model.ShipDelayDateP.HasValue)
            {
                model.ShipDelayDateP = TimeZoneInfo.ConvertTime(model.ShipDelayDateP.Value, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
            }
            if (model.ShipDelayDateNP.HasValue)
            {
                model.ShipDelayDateNP = TimeZoneInfo.ConvertTime(model.ShipDelayDateNP.Value, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
            }

            await model.Skus.AddRangeAsync(dynamic.Skus?.Select(async sku =>
            {
                var result = await _skuMapper.ToModelAsync<SkuEmailItem>(sku.Sku);
                await _productMapper.UpdateModelAsync(result, sku.Sku.Product);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity*sku.Amount;

                result.GeneratedGCCodes = sku.GcsGenerated?.Select(s => s.Code).ToList();

                return result;
            }) ?? Enumerable.Empty<Task<SkuEmailItem>>());

            model.Skus.ForEach(p =>
            {
                p.DisplayName = p.DisplayName ?? String.Empty;
                if (!string.IsNullOrEmpty(p.SubTitle))
                {
                    p.DisplayName += " " + p.SubTitle;
                }
                p.DisplayName += $" ({p.PortionsCount})";
            });

            await model.PromoSkus.AddRangeAsync(dynamic.PromoSkus?.Where(p => p.Enabled).Select(async sku =>
            {
                var result = await _skuMapper.ToModelAsync<SkuEmailItem>(sku.Sku);
                await _productMapper.UpdateModelAsync(result, sku.Sku.Product);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity*sku.Amount;

                result.GeneratedGCCodes = sku.GcsGenerated?.Select(s => s.Code).ToList();

                return result;
            }) ?? Enumerable.Empty<Task<SkuEmailItem>>());

            model.PromoSkus.ForEach(p =>
            {
                p.DisplayName = p.DisplayName ?? String.Empty;
                if (!string.IsNullOrEmpty(p.SubTitle))
                {
                    p.DisplayName += " " + p.SubTitle;
                }
                p.DisplayName += $" ({p.PortionsCount})";
            });

            if (model.AutoShipFrequency.HasValue && model.Skus.Count>0)
            {
                model.AutoShipFrequencyProductName = model.Skus.First().DisplayName;
            }

            if (dynamic.PaymentMethod.IdObjectType == (int)PaymentMethodType.NoCharge && dynamic.Customer.ProfileAddress != null)
            {
                model.BillToAddress = await _addressMapper.ToModelAsync<AddressEmailItem>(dynamic.Customer.ProfileAddress);
                model.BillToAddress.Country = _countryNameCodeResolver.GetCountryName(dynamic.Customer.ProfileAddress);
                model.BillToAddress.StateCodeOrCounty = _countryNameCodeResolver.GetRegionOrStateCode(dynamic.Customer.ProfileAddress);
            }
            else if (dynamic.PaymentMethod?.Address != null)
            {
                model.BillToAddress = await _addressMapper.ToModelAsync<AddressEmailItem>(dynamic.PaymentMethod.Address);
                model.BillToAddress.Country = _countryNameCodeResolver.GetCountryName(dynamic.PaymentMethod.Address);
                model.BillToAddress.StateCodeOrCounty = _countryNameCodeResolver.GetRegionOrStateCode(dynamic.PaymentMethod.Address);
            }

            if (dynamic.ShippingAddress != null)
            {
                model.ShipToAddress = await _addressMapper.ToModelAsync<AddressEmailItem>(dynamic.ShippingAddress);
                model.ShipToAddress.Country = _countryNameCodeResolver.GetCountryName(dynamic.ShippingAddress);
                model.ShipToAddress.StateCodeOrCounty = _countryNameCodeResolver.GetRegionOrStateCode(dynamic.ShippingAddress);
                model.DeliveryInstructions = model.ShipToAddress.DeliveryInstructions;
            }

            switch (dynamic.PaymentMethod.IdObjectType)
            {
                case (int)PaymentMethodType.CreditCard:
                    string cartType = string.Empty;
                    switch ((int)dynamic.PaymentMethod.SafeData.CardType)
                    {
                        case (int)CreditCardType.MasterCard:
                            cartType = "MasterCard";
                            break;
                        case (int)CreditCardType.Visa:
                            cartType = "VISA";
                            break;
                        case (int)CreditCardType.AmericanExpress:
                            cartType = "American Express";
                            break;
                        case (int)CreditCardType.Discover:
                            cartType = "Discover";
                            break;
                    }
                    model.PaymentTypeMessage = string.Format
                        ("{0:c} will be charged to your {1} card<br/>ending in {2} exp: {3}/{4} <em>after</em> this order is shipped.",
                         model.Total, cartType,
                         dynamic.PaymentMethod.SafeData.CardNumber?.Replace("X", ""),
                         ((DateTime)dynamic.PaymentMethod.SafeData.ExpDate).Month,
                         ((DateTime)dynamic.PaymentMethod.SafeData.ExpDate).Year);
                    break;
                case (int)PaymentMethodType.Check:
                    model.PaymentTypeMessage = $"Payment Method: Check #{dynamic.PaymentMethod.SafeData.CheckNumber}";
                    break;
                case (int)PaymentMethodType.Oac:
                case (int)PaymentMethodType.NoCharge:
                case (int)PaymentMethodType.WireTransfer:
                case (int)PaymentMethodType.Marketing:
                case (int)PaymentMethodType.VCWellnessEmployeeProgram:
                    model.PaymentTypeMessage = string.Format("Payment Method: {0}",
                        _referenceData.PaymentMethods.FirstOrDefault(p => p.Key == dynamic.PaymentMethod.IdObjectType));
                    break;
                default:
                    break;
            }

            if (dynamic.GiftCertificates != null)
            {
                model.GiftCertificatesTotal = dynamic.GiftCertificates.Sum(p => p.Amount);
            }
        }

        public override Task ModelToDynamicAsync(OrderConfirmationEmail model, OrderDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
    }
}
