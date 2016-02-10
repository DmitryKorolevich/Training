using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VC.Admin.Validators.Order;
using VC.Admin.Models.Customer;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using System.Threading.Tasks;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Admin.Models.Orders
{
    public class TrackingInvoiceEntity
    {
        public string TrackingNumber { get; set; }

        public string Sku { get; set; }

        public string ServiceUrl { get; set; }
    }

    public class GCInvoiceEntity
    {
        public string Code { get; set; }

        public decimal Amount { get; set; }
    }

    public class OrderInvoiceModel : BaseModel
    {
        [Map]
        public int Id { get; set; }

        [Map]
        public int IdCustomer { get; set; }

        [Map]
        public DateTime DateCreated { get; set; }

        public AddressModel Shipping { get; set; }

        [Map]
        public int? IdObjectType { get; set; }

        [Map]
        public RecordStatusCode StatusCode { get; set; }

        [Map]
        public OrderStatus OrderStatus { get; set; }

        [Map]
        public bool GiftOrder { get; set; }

        [Map]
        public bool MailOrder { get; set; }

        [Map]
        public string PoNumber { get; set; }

        [Map]
        public string KeyCode { get; set; }

        [Map]
        public int? AutoShipFrequency { get; set; }

        [Map]
        public ShipDelayType? ShipDelayType { get; set; }

        [Map]
        public DateTime? ShipDelayDateP { get; set; }

        [Map]
        public DateTime? ShipDelayDateNP { get; set; }

        [Map]
        public DateTime? ShipDelayDate { get; set; }

        [Map]
        public string OrderNotes { get; set; }

        [Map]
        public string GiftMessage { get; set; }

        [Map]
        public string DeliveryInstructions { get; set; }

        [Map]
        public decimal AlaskaHawaiiSurcharge { get; set; }

        [Map]
        public decimal CanadaSurcharge { get; set; }

        [Map]
        public decimal StandardShippingCharges { get; set; }

        [Map]
        public ShippingUpgradeOption? ShippingUpgradeP { get; set; }

        [Map]
        public ShippingUpgradeOption? ShippingUpgradeNP { get; set; }

        [Map]
        public decimal SurchargeOverride { get; set; }

        [Map]
        public decimal ShippingOverride { get; set; }

        [Map]
        public decimal ShippingTotal { get; set; }

        [Map]
        public PreferredShipMethod? PreferredShipMethod { get; set; }

        [Map]
        public decimal ProductsSubtotal { get; set; }

        [Map]
        public decimal DiscountTotal { get; set; }

        [Map]
        public decimal TaxTotal { get; set; }

        [Map]
        public decimal Total { get; set; }




        public bool ShowVitalChoiceTaxId { get; set; }

        public string InvoiceTypeName { get; set; }

        public string IdEditAgent { get; set; }

        public AddressModel BillingAddress { get; set; }

        public AddressModel ShippingAddress { get; set; }

        public string PaymentTypeName { get; set; }

        public string CreditCardInformationRow { get; set; }

        public bool ShowWholesaleShortView { get; set; }

        public bool ShowWholesaleNormalView { get; set; }

        public string OACFOB { get; set; }

        public string PreferredShipMethodName { get; set; }

        public string OACTerms { get; set; }

        public ICollection<SkuOrderedManageModel> SkuOrdereds { get; set; }

        public ICollection<SkuOrderedManageModel> PromoSkuOrdereds { get; set; }

        public bool ShowTrackingView { get; set; }

        public DateTime DateShipped {get;set;}

        public string ShipVia { get; set; }

        public ICollection<TrackingInvoiceEntity> TrackingEntities { get; set; }

        public ICollection<GCInvoiceEntity> GCs { get; set; }

        public decimal? RefundedManualOverride { get; set; }

        public decimal ShippingSurcharge { get; set; }

        //Only shipping with ovveride and without surcharge part
        public decimal TotalShipping { get; set; }

        public string PDFUrl { get; set; }

        public OrderInvoiceModel()
        {
        }

        public void FillAdditionalFields(OrderDynamic order, CustomerDynamic customer, AdminProfile adminProfile, 
            IAppInfrastructureService appInfrastructureService, TimeZoneInfo pstTimeZoneInfo, ITrackingService trackingService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper, ICountryService countryService)
        {
            if (order.ShippingAddress != null)
            {
                this.ShippingAddress = addressMapper.ToModel<AddressModel>(order.ShippingAddress);
                FillAdditionalAddressFields(this.ShippingAddress, countryService);
            }
            if(order?.PaymentMethod.IdObjectType== (int)PaymentMethodType.NoCharge && customer.ProfileAddress!=null)
            {
                this.BillingAddress = addressMapper.ToModel<AddressModel>(customer.ProfileAddress);
                FillAdditionalAddressFields(this.BillingAddress, countryService);
            }
            else if(order?.PaymentMethod?.Address != null)
            {
                this.BillingAddress = addressMapper.ToModel<AddressModel>(order.PaymentMethod.Address);
                FillAdditionalAddressFields(this.BillingAddress, countryService);
            }
            FillGeneralInfo(order, customer, adminProfile, pstTimeZoneInfo, trackingService);
            FillPaymentInfo(order, customer, appInfrastructureService);
        }

        private void FillAdditionalAddressFields(AddressModel addressModel, ICountryService countryService)
        {
            if (addressModel != null && addressModel.Country != null)
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

        private void FillGeneralInfo(OrderDynamic order, CustomerDynamic customer, AdminProfile adminProfile, 
            TimeZoneInfo pstTimeZoneInfo, ITrackingService trackingService)
        {
            IdCustomer = customer.Id;
            DateCreated = TimeZoneInfo.ConvertTime(DateCreated, TimeZoneInfo.Local, pstTimeZoneInfo);
            IdEditAgent = adminProfile == null ? "--" : adminProfile.AgentId;
            if (order.IdObjectType == (int)OrderType.Refund)
            {
                InvoiceTypeName = "Refund Invoice";
            }
            else if (customer.IdObjectType == (int)CustomerType.Wholesale)
            {
                InvoiceTypeName = "Commercial Invoice";
            }
            else
            {
                InvoiceTypeName = "Retail Invoice";
            }

            if(!string.IsNullOrEmpty(OrderNotes))
            {
                OrderNotes = OrderNotes.Replace("\n", "<br />");
            }
            if (!string.IsNullOrEmpty(GiftMessage))
            {
                GiftMessage = GiftMessage.Replace("\n", "<br />");
            }

            SkuOrdereds = order.Skus?.Select(item => new SkuOrderedManageModel(item)).ToList() ?? new List<SkuOrderedManageModel>();
            PromoSkuOrdereds= new List<SkuOrderedManageModel>();//TODO - add reading promos after adding save to DB by Alex g

            GCs = order.GiftCertificates?.Select(item => new GCInvoiceEntity() {
                Amount = item.Amount,
                Code =item.GiftCertificate.Code,
            }).ToList() ?? new List<GCInvoiceEntity>();

            ShippingSurcharge = AlaskaHawaiiSurcharge + CanadaSurcharge - SurchargeOverride;
            TotalShipping = ShippingTotal - ShippingSurcharge;

            //TODO - add ShowTrackingView and it's fields
            //trackingService.GetServiceUrl
            //TODO - add RefundedManualOverride for refund orders
        }

        private void FillPaymentInfo(OrderDynamic order, CustomerDynamic customer, IAppInfrastructureService appInfrastructureService)
        {
            if (order.IdObjectType == (int)OrderType.Refund)
            {

            }
            else
            {
                if (customer.IdObjectType == (int)CustomerType.Wholesale && order?.PaymentMethod?.IdObjectType != (int)PaymentMethodType.Oac)
                {
                    ShowVitalChoiceTaxId = true;
                    ShowWholesaleShortView = true;
                }
            }

            if (order.PaymentMethod != null)
            {
                var paymentMethodInfo = appInfrastructureService.Get().PaymentMethods.FirstOrDefault(p => p.Key == order.PaymentMethod.IdObjectType);
                PaymentTypeName = paymentMethodInfo?.Text;
                switch ((PaymentMethodType)order.PaymentMethod.IdObjectType)
                {
                    case PaymentMethodType.CreditCard:
                        if (order.PaymentMethod.DictionaryData.ContainsKey("CardNumber") && order.PaymentMethod.DictionaryData.ContainsKey("ExpDate"))
                        {
                            if (!string.IsNullOrEmpty(order.PaymentMethod.SafeData.CardNumber) && order.PaymentMethod.SafeData.ExpDate is DateTime)
                            {
                                CreditCardInformationRow = order.PaymentMethod.SafeData.CardNumber;
                                CreditCardInformationRow = CreditCardInformationRow.Replace("X", "");
                                CreditCardInformationRow = String.Format("{0} exp {1}/{2}", CreditCardInformationRow, ((DateTime)order.PaymentMethod.SafeData.ExpDate).Month,
                                    ((DateTime)order.PaymentMethod.SafeData.ExpDate).Year);
                            }
                        }
                        break;
                    case PaymentMethodType.Oac:
                        ShowVitalChoiceTaxId = true;
                        ShowWholesaleNormalView = true;
                        if (PreferredShipMethod.HasValue)
                        {
                            PreferredShipMethodName = appInfrastructureService.Get().OrderPreferredShipMethod.FirstOrDefault(p => p.Key == (int)PreferredShipMethod.Value)?.Text;
                        }
                        if (order.PaymentMethod.DictionaryData.ContainsKey("Fob") && order.PaymentMethod.SafeData.Fob is int)
                        {
                            OACFOB = appInfrastructureService.Get().OacFob.FirstOrDefault(p => p.Key == (int)order.PaymentMethod.SafeData.Fob)?.Text;
                        }
                        if (order.PaymentMethod.DictionaryData.ContainsKey("Terms") && order.PaymentMethod.SafeData.Terms is int)
                        {
                            OACTerms = appInfrastructureService.Get().OacTerms.FirstOrDefault(p => p.Key == (int)order.PaymentMethod.SafeData.Terms)?.Text;
                        }
                        break;
                }
            }
        }

    }
}