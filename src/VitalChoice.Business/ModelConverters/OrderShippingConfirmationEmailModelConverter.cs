using System;
using System.Linq;
using System.Threading.Tasks;
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
            IOptions<AppOptions> options)
        {
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            _countryService = countryService;
            _customerService = customerService;
            _addressMapper = addressMapper;
            _options = options;
        }

        public override async Task DynamicToModelAsync(OrderShippingConfirmationEmail model, OrderDynamic dynamic)
        {
            var countries = await _countryService.GetCountriesAsync();

            dynamic.Customer = await _customerService.SelectAsync(dynamic.Customer.Id);
            model.PublicHost = _options.Value.PublicHost;
            
            model.Email = dynamic.Customer.Email;

            //TODO - fill tracking info
            //TODO: apply some logic for filtering tracking info based on the SendSide field
            //model.Carrier
            //model.ServiceUrl(generating urls - TrackingService.GetServiceUrl)
            //model.TrackingInfoItems
            if (dynamic.SendSide.HasValue)
            {
                model.IsPerishable = dynamic.SendSide == (int)POrderType.P;
            }
            else
            {
                model.IsPerishable = dynamic.SafeData.POrderType == (int)POrderType.P || dynamic.SafeData.POrderType == (int)POrderType.PNP;
            }

            //Dates in the needed timezone
            model.DateCreated = TimeZoneInfo.ConvertTime(model.DateCreated, TimeZoneInfo.Local, _pstTimeZoneInfo);

            if (dynamic.PaymentMethod.IdObjectType == (int)PaymentMethodType.NoCharge && dynamic.Customer.ProfileAddress != null)
            {
                model.BillToAddress = await _addressMapper.ToModelAsync<AddressEmailItem>(dynamic.Customer.ProfileAddress);
                model.BillToAddress.Country = countries.FirstOrDefault(p => p.Id == dynamic.Customer.ProfileAddress.IdCountry)?.CountryName;
                model.BillToAddress.StateCodeOrCounty = BusinessHelper.ResolveStateOrCounty(countries, dynamic.Customer.ProfileAddress);
            }
            else if (dynamic.PaymentMethod?.Address != null)
            {
                model.BillToAddress = await _addressMapper.ToModelAsync<AddressEmailItem>(dynamic.PaymentMethod.Address);
                model.BillToAddress.Country = countries.FirstOrDefault(p => p.Id == dynamic.PaymentMethod.Address.IdCountry)?.CountryName;
                model.BillToAddress.StateCodeOrCounty = BusinessHelper.ResolveStateOrCounty(countries, dynamic.PaymentMethod.Address);
            }

            if (dynamic.ShippingAddress != null)
            {
                model.ShipToAddress = await _addressMapper.ToModelAsync<AddressEmailItem>(dynamic.ShippingAddress);
                model.ShipToAddress.Country = countries.FirstOrDefault(p => p.Id == dynamic.ShippingAddress.IdCountry)?.CountryName;
                model.ShipToAddress.StateCodeOrCounty = BusinessHelper.ResolveStateOrCounty(countries, dynamic.ShippingAddress);
            }
        }

        public override Task ModelToDynamicAsync(OrderShippingConfirmationEmail model, OrderDynamic dynamic)
        {
            return Task.Delay(0);
        }
    }
}
