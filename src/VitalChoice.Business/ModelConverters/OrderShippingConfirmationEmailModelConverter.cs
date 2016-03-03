using System;
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
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.Business.ModelConverters
{
    public class OrderShippingConfirmationEmailModelConverter : BaseModelConverter<OrderShippingConfirmationEmail, OrderDynamic>
    {
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ICountryService _countryService;
        private readonly ICustomerService _customerService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IOptions<AppOptions> _options;

        public OrderShippingConfirmationEmailModelConverter(
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
            _addressMapper = addressMapper;
            _options = options;
        }

        public override void DynamicToModel(OrderShippingConfirmationEmail model, OrderDynamic dynamic)
        {
            var countries = _countryService.GetCountriesAsync().Result;

            dynamic.Customer = _customerService.SelectAsync(dynamic.Customer.Id).Result;
            model.PublicHost = _options.Value.PublicHost;

            model.IsPerishable = dynamic.SafeData.POrderType == (int)POrderType.P || dynamic.SafeData.POrderType == (int)POrderType.PNP;
            model.Email = dynamic.Customer.Email;

            //TODO - fill tracking info
            //model.Carrier
            //model.ServiceUrl(generating urls - TrackingService.GetServiceUrl)
            //model.TrackingInfoItems

            //Dates in the needed timezone
            model.DateCreated = TimeZoneInfo.ConvertTime(model.DateCreated, TimeZoneInfo.Local, _pstTimeZoneInfo);

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
        }

        public override void ModelToDynamic(OrderShippingConfirmationEmail model, OrderDynamic dynamic)
        {
        }
    }
}
