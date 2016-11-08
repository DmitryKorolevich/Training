using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VitalChoice.Business.Queries.Product;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VitalChoice.Business.Queries.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Infrastructure.Domain;

namespace VC.Admin.ModelConverters
{
    public class OrderInvoiceModelConverter : BaseModelConverter<OrderInvoiceModel, OrderDynamic>
    {
        private readonly ITrackingService _trackingService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IAdminUserService _adminUserService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly ReferenceData _referenceData;

        public OrderInvoiceModelConverter(
            ITrackingService trackingService,
            ICustomerService customerService,
            ICountryService countryService,
            IAdminUserService adminUserService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper, ReferenceData referenceData)
        {
            _trackingService = trackingService;
            _customerService = customerService;
            _countryService = countryService;
            _adminUserService = adminUserService;
            _addressMapper = addressMapper;
            _referenceData = referenceData;
        }

        public override async Task DynamicToModelAsync(OrderInvoiceModel model, OrderDynamic dynamic)
        {
            dynamic.Customer = await _customerService.SelectAsync(dynamic.Customer.Id);

            AdminProfile adminProfile = null;
            if (dynamic.IdEditedBy.HasValue)
            {
                adminProfile = await _adminUserService.GetAdminProfileAsync(dynamic.IdEditedBy.Value);
            }

            if (dynamic.ShippingAddress != null)
            {
                model.ShippingAddress = await _addressMapper.ToModelAsync<AddressModel>(dynamic.ShippingAddress);
                await FillAdditionalAddressFields(model.ShippingAddress, _countryService);
                model.DeliveryInstructions = model.ShippingAddress.DeliveryInstructions;
            }
            if (dynamic.PaymentMethod?.Address != null)
            {
                model.BillingAddress = await _addressMapper.ToModelAsync<AddressModel>(dynamic.PaymentMethod.Address);
                await FillAdditionalAddressFields(model.BillingAddress, _countryService);
            }

            model.IdCustomer = dynamic.Customer.Id;
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
            model.IdEditAgent = adminProfile == null ? "--" : adminProfile.AgentId;
            if (dynamic.IdObjectType == (int)OrderType.Refund)
            {
                model.InvoiceTypeName = "Refund Invoice";
            }
            else if (dynamic.Customer.IdObjectType == (int)CustomerType.Wholesale)
            {
                model.InvoiceTypeName = "Commercial Invoice";
            }
            else
            {
                model.InvoiceTypeName = "Retail Invoice";
            }

            if (!string.IsNullOrEmpty(model.OrderNotes))
            {
                model.OrderNotes = model.OrderNotes.Replace("\n", "<br />");
            }
            if (!string.IsNullOrEmpty(model.GiftMessage))
            {
                model.GiftMessage = model.GiftMessage.Replace("\n", "<br />");
            }

            model.SkuOrdereds = dynamic.Skus?.Select(item => new SkuOrderedManageModel(item)).ToList() ?? new List<SkuOrderedManageModel>();
            model.PromoSkuOrdereds = dynamic.PromoSkus?.Where(p => p.Enabled).Select(item => new SkuOrderedManageModel(item)).ToList() ?? new List<SkuOrderedManageModel>();

            model.GCs = dynamic.GiftCertificates?.Select(item => new GCInvoiceItemModel()
            {
                Amount = item.Amount,
                Code = item.GiftCertificate.Code,
            }).ToList() ?? new List<GCInvoiceItemModel>();

            model.ShippingSurcharge = model.AlaskaHawaiiSurcharge + model.CanadaSurcharge - model.SurchargeOverride;
            model.TotalShipping = model.ShippingTotal - model.ShippingSurcharge;

            if (dynamic.OrderShippingPackages != null && dynamic.OrderShippingPackages.Count > 0)
            {
                model.ShowTrackingView = true;

                var package = dynamic.OrderShippingPackages.FirstOrDefault(p => !p.POrderType.HasValue);
                if (package != null)
                {
                    model.DateShipped = package.ShippedDate;
                    model.ShipVia = $"{package.ShipMethodFreightCarrier} - {package.ShipMethodFreightService}";
                }
                model.TrackingEntities = dynamic.OrderShippingPackages.Where(p=>!p.POrderType.HasValue).Select(p => new TrackingInvoiceItemModel()
                {
                    Sku = p.UPSServiceCode,
                    ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier, p.TrackingNumber),
                    TrackingNumber = p.TrackingNumber,
                }).ToList();

                package = dynamic.OrderShippingPackages.FirstOrDefault(p => p.POrderType==(int)POrderType.P);
                if (package != null)
                {
                    model.PDateShipped = package.ShippedDate;
                    model.PShipVia = $"{package.ShipMethodFreightCarrier} - {package.ShipMethodFreightService}";
                }
                model.PTrackingEntities = dynamic.OrderShippingPackages.Where(p => p.POrderType == (int)POrderType.P).Select(p => new TrackingInvoiceItemModel()
                {
                    Sku = p.UPSServiceCode,
                    ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier, p.TrackingNumber),
                    TrackingNumber = p.TrackingNumber,
                }).ToList();

                package = dynamic.OrderShippingPackages.FirstOrDefault(p => p.POrderType== (int)POrderType.NP);
                if (package != null)
                {
                    model.NPDateShipped = package.ShippedDate;
                    model.NPShipVia = $"{package.ShipMethodFreightCarrier} - {package.ShipMethodFreightService}";
                }
                model.NPTrackingEntities = dynamic.OrderShippingPackages.Where(p => p.POrderType == (int)POrderType.NP).Select(p => new TrackingInvoiceItemModel()
                {
                    Sku = p.UPSServiceCode,
                    ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier, p.TrackingNumber),
                    TrackingNumber = p.TrackingNumber,
                }).ToList();
            }

            if (dynamic.IdObjectType == (int)OrderType.Refund)
            {
                model.RefundedManualOverride = dynamic.SafeData.ManualRefundOverride;
            }
            else
            {
                if (dynamic.Customer.IdObjectType == (int)CustomerType.Wholesale && dynamic.PaymentMethod?.IdObjectType != (int)PaymentMethodType.Oac)
                {
                    model.ShowVitalChoiceTaxId = true;
                    model.ShowWholesaleShortView = true;
                }
            }

            if (dynamic.PaymentMethod != null)
            {
                var paymentMethodInfo = _referenceData.PaymentMethods.FirstOrDefault(p => p.Key == dynamic.PaymentMethod.IdObjectType);
                model.PaymentTypeName = paymentMethodInfo?.Text;
                switch ((PaymentMethodType)dynamic.PaymentMethod.IdObjectType)
                {
                    case PaymentMethodType.CreditCard:
                        if (dynamic.PaymentMethod.DictionaryData.ContainsKey("CardNumber") && dynamic.PaymentMethod.DictionaryData.ContainsKey("ExpDate"))
                        {
                            if (!string.IsNullOrEmpty(dynamic.PaymentMethod.SafeData.CardNumber) && dynamic.PaymentMethod.SafeData.ExpDate is DateTime)
                            {
                                model.CreditCardInformationRow = dynamic.PaymentMethod.SafeData.CardNumber;
                                model.CreditCardInformationRow = model.CreditCardInformationRow.Replace("X", "");
                                model.CreditCardInformationRow =
                                    $"{model.CreditCardInformationRow} exp {((DateTime) dynamic.PaymentMethod.SafeData.ExpDate).Month}/{((DateTime) dynamic.PaymentMethod.SafeData.ExpDate).Year}";
                            }
                        }
                        break;
                    case PaymentMethodType.Oac:
                        model.ShowVitalChoiceTaxId = true;
                        model.ShowWholesaleNormalView = true;
                        if (model.ShippingAddress.PreferredShipMethod.HasValue)
                        {
                            model.PreferredShipMethodName = _referenceData.OrderPreferredShipMethod.FirstOrDefault(p => p.Key == (int)model.ShippingAddress.PreferredShipMethod.Value)?.Text;
                        }
                        if (dynamic.PaymentMethod.DictionaryData.ContainsKey("Fob") && dynamic.PaymentMethod.SafeData.Fob is int)
                        {
                            model.OACFOB = _referenceData.OacFob.FirstOrDefault(p => p.Key == (int)dynamic.PaymentMethod.SafeData.Fob)?.Text;
                        }
                        if (dynamic.PaymentMethod.DictionaryData.ContainsKey("Terms") && dynamic.PaymentMethod.SafeData.Terms is int)
                        {
                            model.OACTerms = _referenceData.OacTerms.FirstOrDefault(p => p.Key == (int)dynamic.PaymentMethod.SafeData.Terms)?.Text;
                        }
                        break;
                }
            }
        }

        private async Task FillAdditionalAddressFields(AddressModel addressModel, ICountryService countryService)
        {
            if (addressModel?.Country != null)
            {
                var country = await countryService.GetCountryAsync(addressModel.Country.Id);
                if (country != null)
                {
                    addressModel.Country.CountryCode = country.CountryCode;
                    if (addressModel.State != 0)
                    {
                        var state = country.States.FirstOrDefault(p => p.Id == addressModel.State);
                        if (state != null)
                        {
                            addressModel.StateCode = state.StateCode;
                        }
                    }
                }
                addressModel.Phone = addressModel.Phone.FormatAsPhone(BaseAppConstants.BASE_PHONE_FORMAT);
                addressModel.Fax = addressModel.Fax.FormatAsPhone(BaseAppConstants.BASE_PHONE_FORMAT);
            }
        }

        public override Task ModelToDynamicAsync(OrderInvoiceModel model, OrderDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
        
    }
}