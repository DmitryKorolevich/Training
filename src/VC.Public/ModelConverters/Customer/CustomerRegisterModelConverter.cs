using System;
using VC.Public.Models.Auth;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Public.ModelConverters.Customer
{
    public class CustomerRegisterModelConverter : BaseModelConverter<RegisterAccountModel, CustomerDynamic>
    {
        private readonly IDynamicMapper<AddressDynamic, Address> _addressMapper;

        public CustomerRegisterModelConverter(IDynamicMapper<AddressDynamic, Address> addressMapper)
        {
            _addressMapper = addressMapper;
        }

        public override void DynamicToModel(RegisterAccountModel model, CustomerDynamic dynamic)
	    {
		    throw new NotImplementedException();
		}

	    public override void ModelToDynamic(RegisterAccountModel model, CustomerDynamic dynamic)
	    {
			var addressDynamic = _addressMapper.FromModel(model);
				
			addressDynamic.IdObjectType = (int)AddressType.Profile;
			dynamic.ShippingAddresses.Add(addressDynamic);

		    var shippngAddress = _addressMapper.FromModel(model);
            shippngAddress.IdObjectType = (int)AddressType.Shipping;
		    shippngAddress.Data.Default = true;
			dynamic.ShippingAddresses.Add(shippngAddress);

			dynamic.StatusCode = (int)RecordStatusCode.Active;
	    }
	}
}
