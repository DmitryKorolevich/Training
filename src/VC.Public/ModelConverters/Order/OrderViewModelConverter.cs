using VC.Public.Helpers;
using VC.Public.Models.Profile;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
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

        public OrderViewModelConverter(
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICountryService countryService,
            IAppInfrastructureService appInfrastructureService)
        {
            _addressMapper = addressMapper;
            _countryService = countryService;
            _referenceData = appInfrastructureService.Get();
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
        }

        public override void ModelToDynamic(OrderViewModel model, OrderDynamic dynamic)
        {
        }
    }
}
