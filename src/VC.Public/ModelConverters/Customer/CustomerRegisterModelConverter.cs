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
			var profileAddress = _addressMapper.FromModel(model);
				
			profileAddress.IdObjectType = (int)AddressType.Profile;
			dynamic.ProfileAddress = profileAddress;

		    var shippingAddress = _addressMapper.FromModel(model);
            shippingAddress.IdObjectType = (int)AddressType.Shipping;
		    shippingAddress.Data.Default = true;
			dynamic.ShippingAddresses.Add(shippingAddress);

			dynamic.StatusCode = (int)RecordStatusCode.Active;
	    }
	}
}
