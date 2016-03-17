﻿using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Models.Customer;
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
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Admin.ModelConverters
{
    public class OrderInvoiceModelConverter : BaseModelConverter<OrderInvoiceModel, OrderDynamic>
    {
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly ITrackingService _trackingService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IAdminUserService _adminUserService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly TimeZoneInfo _pstTimeZoneInfo;

        public OrderInvoiceModelConverter(IAppInfrastructureService appInfrastructureService, 
            ITrackingService trackingService,
            ICustomerService customerService,
            ICountryService countryService,
            IAdminUserService adminUserService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper)
        {
            _appInfrastructureService = appInfrastructureService;
            _trackingService = trackingService;
            _customerService = customerService;
            _countryService = countryService;
            _adminUserService = adminUserService;
            _addressMapper = addressMapper;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        public override void DynamicToModel(OrderInvoiceModel model, OrderDynamic dynamic)
        {
            dynamic.Customer = _customerService.SelectAsync(dynamic.Customer.Id).Result;

            AdminProfile adminProfile = null;
            if (dynamic.IdEditedBy.HasValue)
            {
                adminProfile = _adminUserService.GetAdminProfileAsync(dynamic.IdEditedBy.Value).Result;
            }

            if (dynamic.ShippingAddress != null)
            {
                model.ShippingAddress = _addressMapper.ToModel<AddressModel>(dynamic.ShippingAddress);
                FillAdditionalAddressFields(model.ShippingAddress, _countryService);
            }
            if (dynamic?.PaymentMethod.IdObjectType == (int)PaymentMethodType.NoCharge && dynamic.Customer.ProfileAddress != null)
            {
                model.BillingAddress = _addressMapper.ToModel<AddressModel>(dynamic.Customer.ProfileAddress);
                FillAdditionalAddressFields(model.BillingAddress, _countryService);
            }
            else if (dynamic?.PaymentMethod?.Address != null)
            {
                model.BillingAddress = _addressMapper.ToModel<AddressModel>(dynamic.PaymentMethod.Address);
                FillAdditionalAddressFields(model.BillingAddress, _countryService);
            }

            model.IdCustomer = dynamic.Customer.Id;
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

            //TODO - add ShowTrackingView and it's fields
            //trackingService.GetServiceUrl
            //TODO - add RefundedManualOverride for refund orders

            if (dynamic.IdObjectType == (int)OrderType.Refund)
            {
                //TODO - add showing info for refund orders
            }
            else
            {
                if (dynamic.Customer.IdObjectType == (int)CustomerType.Wholesale && dynamic?.PaymentMethod?.IdObjectType != (int)PaymentMethodType.Oac)
                {
                    model.ShowVitalChoiceTaxId = true;
                    model.ShowWholesaleShortView = true;
                }
            }

            if (dynamic.PaymentMethod != null)
            {
                var paymentMethodInfo = _appInfrastructureService.Get().PaymentMethods.FirstOrDefault(p => p.Key == dynamic.PaymentMethod.IdObjectType);
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
                        if (model.PreferredShipMethod.HasValue)
                        {
                            model.PreferredShipMethodName = _appInfrastructureService.Get().OrderPreferredShipMethod.FirstOrDefault(p => p.Key == (int)model.PreferredShipMethod.Value)?.Text;
                        }
                        if (dynamic.PaymentMethod.DictionaryData.ContainsKey("Fob") && dynamic.PaymentMethod.SafeData.Fob is int)
                        {
                            model.OACFOB = _appInfrastructureService.Get().OacFob.FirstOrDefault(p => p.Key == (int)dynamic.PaymentMethod.SafeData.Fob)?.Text;
                        }
                        if (dynamic.PaymentMethod.DictionaryData.ContainsKey("Terms") && dynamic.PaymentMethod.SafeData.Terms is int)
                        {
                            model.OACTerms = _appInfrastructureService.Get().OacTerms.FirstOrDefault(p => p.Key == (int)dynamic.PaymentMethod.SafeData.Terms)?.Text;
                        }
                        break;
                }
            }
        }

        private void FillAdditionalAddressFields(AddressModel addressModel, ICountryService countryService)
        {
            if (addressModel?.Country != null)
            {
                var country = countryService.GetCountryAsync(addressModel.Country.Id).Result;
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

        public override void ModelToDynamic(OrderInvoiceModel model, OrderDynamic dynamic)
        {
        
        }
        
    }
}