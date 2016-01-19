using System;
using VC.Public.Models.Auth;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
    public class RegisterWholesaleAccountModelConverter : BaseModelConverter<RegisterWholesaleAccountModel, CustomerDynamic>
    {
        private readonly IDynamicMapper<AddressDynamic, Address> _addressMapper;

        public RegisterWholesaleAccountModelConverter(IDynamicMapper<AddressDynamic, Address> addressMapper)
        {
            _addressMapper = addressMapper;
        }

        public override void DynamicToModel(RegisterWholesaleAccountModel model, CustomerDynamic dynamic)
	    {
		    throw new NotImplementedException();
		}

	    public override void ModelToDynamic(RegisterWholesaleAccountModel model, CustomerDynamic dynamic)
	    {
			var profileAddress = _addressMapper.FromModel(model);
				
			profileAddress.IdObjectType = (int)AddressType.Profile;
			dynamic.ProfileAddress = profileAddress;

		    var shippingAddress = _addressMapper.FromModel(model);
            shippingAddress.IdObjectType = (int)AddressType.Shipping;
		    shippingAddress.Data.Default = true;
			dynamic.ShippingAddresses.Add(shippingAddress);
            
            dynamic.Data.TradeClass = model.TradeClass;
            dynamic.Data.PromotingWebsites = model.PromotingWebsites;
	    }
	}
}
