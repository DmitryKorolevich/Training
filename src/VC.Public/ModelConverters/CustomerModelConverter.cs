using System;
using System.Linq;
using VC.Public.Models.Auth;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.Utils;

namespace VC.Public.ModelConverters
{
    public class CustomerModelConverter : IModelToDynamicConverter<RegisterAccountModel, CustomerDynamic>
    {
        private readonly IDynamicToModelMapper<CustomerAddressDynamic> _addressMapper;

        public CustomerModelConverter(IDynamicToModelMapper<CustomerAddressDynamic> addressMapper)
        {
            _addressMapper = addressMapper;
        }

        public void DynamicToModel(RegisterAccountModel model, CustomerDynamic dynamic)
	    {
		    throw new NotImplementedException();
		}

	    public void ModelToDynamic(RegisterAccountModel model, CustomerDynamic dynamic)
	    {
			var addressDynamic = _addressMapper.FromModel(model);
				
			addressDynamic.IdObjectType = (int)AddressType.Profile;
			dynamic.Addresses.Add(addressDynamic);

		    var shippngAddress = _addressMapper.FromModel(model);
            shippngAddress.IdObjectType = (int)AddressType.Shipping;
		    shippngAddress.Data.Default = true;
			dynamic.Addresses.Add(shippngAddress);

			dynamic.StatusCode = RecordStatusCode.Active;
	    }
	}
}
