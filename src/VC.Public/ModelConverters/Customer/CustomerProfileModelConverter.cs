using System;
using System.Linq;
using VC.Public.Models.Auth;
using VC.Public.Models.Profile;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Public.ModelConverters.Customer
{
    public class CustomerProfileModelConverter : IModelToDynamicConverter<ChangeProfileModel, CustomerDynamic>
    {
        private readonly IDynamicToModelMapper<CustomerAddressDynamic> _addressMapper;

        public CustomerProfileModelConverter(IDynamicToModelMapper<CustomerAddressDynamic> addressMapper)
        {
            _addressMapper = addressMapper;
        }

        public void DynamicToModel(ChangeProfileModel model, CustomerDynamic dynamic)
	    {
			model = _addressMapper.ToModel<ChangeProfileModel>(dynamic.Addresses.Single(x=> x.IdObjectType == (int)AddressType.Profile));
		}

	    public void ModelToDynamic(ChangeProfileModel model, CustomerDynamic dynamic)
	    {
			var addressDynamic = _addressMapper.FromModel(model);
				
			addressDynamic.IdObjectType = (int)AddressType.Profile;

			var oldAddress = dynamic.Addresses.Single(x=>x.IdObjectType == (int)AddressType.Profile);
		    dynamic.Addresses.Remove(oldAddress);

			dynamic.Addresses.Add(addressDynamic);
	    }
	}
}
