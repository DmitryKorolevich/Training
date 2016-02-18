using System;
using System.Collections.Generic;
using System.Linq;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;

namespace VC.Public.ModelConverters.Order
{
    public class OrderViewModelConverter : BaseModelConverter<OrderViewModel, OrderDynamic>
    {
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ICountryService _countryService;
        private readonly ReferenceData _referenceData;
        protected readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        protected readonly IDynamicMapper<ProductDynamic, Product> _productMapper;

        public OrderViewModelConverter(
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICountryService countryService,
            IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<SkuDynamic, Sku> skuMapper,
            IDynamicMapper<ProductDynamic, Product> productMapper)
        {
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            _countryService = countryService;
            _referenceData = appInfrastructureService.Get();
            _skuMapper = skuMapper;
            _productMapper = productMapper;
        }

        public override void DynamicToModel(OrderViewModel model, OrderDynamic dynamic)
        {
            var countries = _countryService.GetCountriesAsync().Result;
            if (dynamic?.PaymentMethod.IdObjectType == (int)PaymentMethodType.NoCharge && dynamic.Customer.ProfileAddress != null)
            {
                model.BillToAddress = dynamic.Customer.ProfileAddress.PopulateBillingAddressDetails(countries,
                    dynamic.Customer.Email);
            }
            else if (dynamic?.PaymentMethod?.Address != null)
            {
                model.BillToAddress = dynamic.PaymentMethod.Address.PopulateBillingAddressDetails(countries,
                    dynamic.Customer.Email);
            }

            if (dynamic?.PaymentMethod?.IdObjectType == (int)PaymentMethodType.CreditCard)
            {
                model.CreditCardDetails = dynamic.PaymentMethod.PopulateCreditCardDetails(_referenceData, true);
            }

            if (dynamic?.ShippingAddress != null)
            {
                model.ShipToAddress = dynamic.ShippingAddress.PopulateShippingAddressDetails(countries);
            }
            
            model.IdPaymentMethodType = dynamic?.PaymentMethod.IdObjectType;

            model.Skus.AddRange(dynamic?.Skus?.Select(sku =>
                {
                    var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                    _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity * sku.Amount;
                    return result;
                }) ?? Enumerable.Empty<CartSkuModel>());

            model.Skus.ForEach(p =>
            {
                p.DisplayName = p.DisplayName ?? String.Empty;
                if (!string.IsNullOrEmpty(p.SubTitle))
                {
                    p.DisplayName += " " + p.SubTitle;
                }
                p.DisplayName += $" ({p.Quantity})";
            });

            model.PromoSkus.AddRange(dynamic?.PromoSkus?.Select(sku =>
            {
                var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity * sku.Amount;
                return result;
            }) ?? Enumerable.Empty<CartSkuModel>());

            model.PromoSkus.ForEach(p =>
            {
                p.DisplayName = p.DisplayName ?? String.Empty;
                if (!string.IsNullOrEmpty(p.SubTitle))
                {
                    p.DisplayName += " " + p.SubTitle;
                }
                p.DisplayName += $" ({p.PortionsCount})";
            });

            model.ShippingSurcharge = model.AlaskaHawaiiSurcharge + model.CanadaSurcharge - model.SurchargeOverride;
            model.TotalShipping = model.ShippingTotal - model.ShippingSurcharge;

            if (dynamic?.GiftCertificates != null)
            {
                model.GiftCertificatesTotal = dynamic.GiftCertificates.Sum(p => p.Amount);
            }

            if (dynamic?.Discount != null)
            {
                model.DiscountCode = dynamic.Discount.Code;
                model.DiscountCodeMessage = dynamic.Discount.GetDiscountMessage((int?)dynamic.SafeData.IdDiscountTier);
            }

            model.GCs = dynamic?.GiftCertificates?.Select(item => new GCInvoiceEntity()
            {
                Amount = item.Amount,
                Code = item.GiftCertificate.Code,
            }).ToList() ?? new List<GCInvoiceEntity>();

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
        }

        public override void ModelToDynamic(OrderViewModel model, OrderDynamic dynamic)
        {
        }
    }
}
