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
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.Business.ModelConverters
{
    public class OrderShippingConfirmationEmailModelConverter : BaseModelConverter<OrderShippingConfirmationEmail, OrderDynamic>
    {
        private readonly ICustomerService _customerService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IOptions<AppOptions> _options;
        private readonly ITrackingService _trackingService;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public OrderShippingConfirmationEmailModelConverter(
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICustomerService customerService,
            IOptions<AppOptions> options,
            ITrackingService trackingService, ICountryNameCodeResolver countryNameCodeResolver)
        {
            _customerService = customerService;
            _addressMapper = addressMapper;
            _options = options;
            _trackingService = trackingService;
            _countryNameCodeResolver = countryNameCodeResolver;
        }

        public override async Task DynamicToModelAsync(OrderShippingConfirmationEmail model, OrderDynamic dynamic)
        {
            dynamic.Customer = await _customerService.SelectAsync(dynamic.Customer.Id);
            model.PublicHost = _options.Value.PublicHost;
            
            model.Email = dynamic.Customer.Email;

            var packageInfo =
                dynamic.OrderShippingPackages.Where(p => p.POrderType == dynamic.SendSide)
                    .Select(p => new
                    {
                        p.ShipMethodFreightCarrier,
                        p.ShipMethodFreightService,
                        p.TrackingNumber,
                    }).Distinct().ToList();

            model.TrackingInfoItems = packageInfo.Select(p => new TrackingInfoEmailItem()
            {
                Number = p.TrackingNumber,
                ServiceUrl = _trackingService.GetServiceUrl(p.ShipMethodFreightCarrier,p.TrackingNumber),
            }).ToList();
            if (packageInfo.Any())
            {
                model.Carrier = packageInfo.First().ShipMethodFreightCarrier;
                model.ServiceUrl = _trackingService.GetServiceUrl(packageInfo.First().ShipMethodFreightCarrier,
                    packageInfo.First().TrackingNumber);
            }
            if (dynamic.SendSide.HasValue)
            {
                model.IsPerishable = dynamic.SendSide == (int)POrderType.P;
            }
            else
            {
                model.IsPerishable = dynamic.SafeData.POrderType == (int)POrderType.P || dynamic.SafeData.POrderType == (int)POrderType.PNP;
            }

            //Dates in the needed timezone
            model.DateCreated = TimeZoneInfo.ConvertTime(model.DateCreated, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);

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
            }
        }

        public override Task ModelToDynamicAsync(OrderShippingConfirmationEmail model, OrderDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
    }
}
