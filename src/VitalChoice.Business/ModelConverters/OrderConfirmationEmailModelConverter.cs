﻿using System;
using System.Linq;
using Microsoft.Extensions.OptionsModel;
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
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly ReferenceData _referenceData;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IOptions<AppOptions> _options;

        public OrderConfirmationEmailModelConverter(
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICountryService countryService,
            ICustomerService customerService,
            IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<SkuDynamic, Sku> skuMapper,
            IDynamicMapper<ProductDynamic, Product> productMapper,
            IOptions<AppOptions> options)
        {
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            _countryService = countryService;
            _customerService = customerService;
            _referenceData = appInfrastructureService.Get();
            _addressMapper = addressMapper;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _options = options;
        }

        public override void DynamicToModel(OrderConfirmationEmail model, OrderDynamic dynamic)
        {
            var countries = _countryService.GetCountriesAsync().Result;

            dynamic.Customer = _customerService.SelectAsync(dynamic.Customer.Id).Result;
            model.PublicHost = _options.Value.PublicHost;

            //Dates in the needed timezone
            model.DateCreated = TimeZoneInfo.ConvertTime(model.DateCreated, TimeZoneInfo.Local, _pstTimeZoneInfo);
            if (model.ShipDelayDate.HasValue)
            {
                model.ShipDelayDate = TimeZoneInfo.ConvertTime(model.ShipDelayDate.Value, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }
            if (model.ShipDelayDateP.HasValue)
            {
                model.ShipDelayDateP = TimeZoneInfo.ConvertTime(model.ShipDelayDateP.Value, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }
            if (model.ShipDelayDateNP.HasValue)
            {
                model.ShipDelayDateNP = TimeZoneInfo.ConvertTime(model.ShipDelayDateNP.Value, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }

            model.Skus.AddRange(dynamic?.Skus?.Select(sku =>
            {
                var result = _skuMapper.ToModel<SkuEmailItem>(sku.Sku);
                _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity * sku.Amount; 
                return result;
            }) ?? Enumerable.Empty<SkuEmailItem>());

            model.Skus.ForEach(p =>
            {
                p.DisplayName = p.DisplayName ?? String.Empty;
                if (!string.IsNullOrEmpty(p.SubTitle))
                {
                    p.DisplayName += " " + p.SubTitle;
                }
                p.DisplayName += $" ({p.PortionsCount})";
            });

            model.PromoSkus.AddRange(dynamic?.PromoSkus?.Select(sku =>
            {
                var result = _skuMapper.ToModel<SkuEmailItem>(sku.Sku);
                _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity * sku.Amount;
                return result;
            }) ?? Enumerable.Empty<SkuEmailItem>());

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

            if (dynamic?.PaymentMethod.IdObjectType == (int)PaymentMethodType.NoCharge && dynamic.Customer.ProfileAddress != null)
            {
                model.BillToAddress = _addressMapper.ToModel<AddressEmailItem>(dynamic.Customer.ProfileAddress);
                model.BillToAddress.Country = countries.FirstOrDefault(p => p.Id == dynamic.Customer.ProfileAddress.IdCountry)?.CountryName;
                model.BillToAddress.StateCodeOrCounty = DynamicViewHelper.ResolveStateOrCounty(countries, dynamic.Customer.ProfileAddress);
            }
            else if (dynamic?.PaymentMethod?.Address != null)
            {
                model.BillToAddress = _addressMapper.ToModel<AddressEmailItem>(dynamic.PaymentMethod.Address);
                model.BillToAddress.Country = countries.FirstOrDefault(p => p.Id == dynamic.PaymentMethod.Address.IdCountry)?.CountryName;
                model.BillToAddress.StateCodeOrCounty = DynamicViewHelper.ResolveStateOrCounty(countries, dynamic.PaymentMethod.Address);
            }

            if (dynamic?.ShippingAddress != null)
            {
                model.ShipToAddress = _addressMapper.ToModel<AddressEmailItem>(dynamic.ShippingAddress);
                model.ShipToAddress.Country = countries.FirstOrDefault(p => p.Id == dynamic.ShippingAddress.IdCountry)?.CountryName;
                model.ShipToAddress.StateCodeOrCounty = DynamicViewHelper.ResolveStateOrCounty(countries, dynamic.ShippingAddress);
            }

            switch (dynamic?.PaymentMethod.IdObjectType)
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
                        _referenceData.PaymentMethods.FirstOrDefault(p => p.Key == dynamic?.PaymentMethod.IdObjectType));
                    break;
                default:
                    break;
            }
        }

        public override void ModelToDynamic(OrderConfirmationEmail model, OrderDynamic dynamic)
        {
        }
    }
}