using System;
using VC.Public.Models.Auth;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Dynamic;

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
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();

            var profileAddress = _addressMapper.FromModel(model, (int)AddressType.Profile);
				
			dynamic.ProfileAddress = profileAddress;

		    var shippingAddress = _addressMapper.FromModel(model, (int)AddressType.Shipping);
		    shippingAddress.Data.Default = true;
			dynamic.ShippingAddresses.Add(shippingAddress);

			dynamic.StatusCode = (int)RecordStatusCode.Active;
	    }
	}
}
