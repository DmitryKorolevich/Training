using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
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
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.SharedWeb.Helpers;

namespace VC.Public.ModelConverters.Order
{
    public class OrderViewModelConverter : BaseModelConverter<OrderViewModel, OrderDynamic>
    {
        private readonly ReferenceData _referenceData;
        protected readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        protected readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly ITrackingService _trackingService;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public OrderViewModelConverter(
            IDynamicMapper<SkuDynamic, Sku> skuMapper,
            IDynamicMapper<ProductDynamic, Product> productMapper,
            ITrackingService trackingService,
            ICountryNameCodeResolver countryNameCodeResolver, ReferenceData referenceData)
        {
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _trackingService = trackingService;
            _countryNameCodeResolver = countryNameCodeResolver;
            _referenceData = referenceData;
        }

        public override async Task DynamicToModelAsync(OrderViewModel model, OrderDynamic dynamic)
        {
            if (dynamic.PaymentMethod?.Address != null)
            {
                model.BillToAddress = dynamic.PaymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver,
                    dynamic.Customer.Email, true);
            }

            if (dynamic.PaymentMethod?.IdObjectType == (int)PaymentMethodType.CreditCard)
            {
                model.CreditCardDetails = dynamic.PaymentMethod.PopulateCreditCardDetails(_referenceData, true);
            }

            if (dynamic.ShippingAddress != null)
            {
                model.ShipToAddress = dynamic.ShippingAddress.PopulateShippingAddressDetails(_countryNameCodeResolver, true);
                model.DeliveryInstructions = dynamic.ShippingAddress.SafeData.DeliveryInstructions;
            }
            
            model.IdPaymentMethodType = dynamic.PaymentMethod.IdObjectType;

            await model.Skus.AddRangeAsync(dynamic.Skus?.Select(async sku =>
                {
                    var result = await _skuMapper.ToModelAsync<CartSkuModel>(sku.Sku);
                    await _productMapper.UpdateModelAsync(result, sku.Sku.Product);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity * sku.Amount;

                    result.GeneratedGCCodes = sku.GcsGenerated?.Select(g => g.Code).ToList() ?? new List<string>();

                    return result;
                }) ?? Enumerable.Empty<Task<CartSkuModel>>());

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
                var result = await _skuMapper.ToModelAsync<CartSkuModel>(sku.Sku);
                await _productMapper.UpdateModelAsync(result, sku.Sku.Product);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity*sku.Amount;

                return result;
            }) ?? Enumerable.Empty<Task<CartSkuModel>>());

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

            if (dynamic.GiftCertificates != null)
            {
                model.GiftCertificatesTotal = dynamic.GiftCertificates.Sum(p => p.Amount);
            }

            if (dynamic.Discount != null)
            {
                model.DiscountCode = dynamic.Discount.Code;
                model.DiscountCodeMessage = dynamic.Discount.GetDiscountMessage((int?)dynamic.SafeData.IdDiscountTier);
            }

            model.GCs = dynamic.GiftCertificates?.Select(item => new GCInvoiceEntity()
            {
                Amount = item.Amount,
                Code = item.GiftCertificate.Code,
            }).ToList() ?? new List<GCInvoiceEntity>();

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

            if (dynamic.OrderShippingPackages != null && dynamic.OrderShippingPackages.Count > 0)
            {
                var package = dynamic.OrderShippingPackages.FirstOrDefault(p => !p.POrderType.HasValue);
                if (package != null)
                {
                    model.DateShipped = package.ShippedDate;
                    model.ShipVia = $"{package.ShipMethodFreightCarrier} - {package.ShipMethodFreightService}";
                }
                model.TrackingEntities = dynamic.OrderShippingPackages.Where(p => !p.POrderType.HasValue).Select(p => new TrackingItemModel()
                {
                    Sku = p.UPSServiceCode,
                    ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier, p.TrackingNumber),
                    TrackingNumber = p.TrackingNumber,
                }).ToList();

                package = dynamic.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.P);
                if (package != null)
                {
                    model.PDateShipped = package.ShippedDate;
                    model.PShipVia = $"{package.ShipMethodFreightCarrier} - {package.ShipMethodFreightService}";
                }
                model.PTrackingEntities = dynamic.OrderShippingPackages.Where(p => p.POrderType == (int)POrderType.P).Select(p => new TrackingItemModel()
                {
                    Sku = p.UPSServiceCode,
                    ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier, p.TrackingNumber),
                    TrackingNumber = p.TrackingNumber,
                }).ToList();

                package = dynamic.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.NP);
                if (package != null)
                {
                    model.NPDateShipped = package.ShippedDate;
                    model.NPShipVia = $"{package.ShipMethodFreightCarrier} - {package.ShipMethodFreightService}";
                }
                model.NPTrackingEntities = dynamic.OrderShippingPackages.Where(p => p.POrderType == (int)POrderType.NP).Select(p => new TrackingItemModel()
                {
                    Sku = p.UPSServiceCode,
                    ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier, p.TrackingNumber),
                    TrackingNumber = p.TrackingNumber,
                }).ToList();
            }
        }

        public override Task ModelToDynamicAsync(OrderViewModel model, OrderDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
    }
}
