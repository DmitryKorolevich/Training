﻿using System.Linq;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VC.Public.Models.Profile;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
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
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
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
            _addressMapper = addressMapper;
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
                model.ShipToAddress = dynamic.PaymentMethod.Address.PopulateShippingAddressDetails(countries);
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

            model.PromoSkus.AddRange(dynamic?.PromoSkus?.Select(sku =>
            {
                var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity * sku.Amount;
                return result;
            }) ?? Enumerable.Empty<CartSkuModel>());
        }

        public override void ModelToDynamic(OrderViewModel model, OrderDynamic dynamic)
        {
        }
    }
}
