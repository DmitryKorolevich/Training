using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Auth;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain;

namespace VC.Public.ModelConverters.Customer
{
    public class RegisterWholesaleAccountModelConverter : BaseModelConverter<RegisterWholesaleAccountModel, CustomerDynamic>
    {
        private readonly IDynamicMapper<AddressDynamic, Address> _addressMapper;

        public RegisterWholesaleAccountModelConverter(IDynamicMapper<AddressDynamic, Address> addressMapper)
        {
            _addressMapper = addressMapper;
        }

        public override Task DynamicToModelAsync(RegisterWholesaleAccountModel model, CustomerDynamic dynamic)
	    {
            return TaskCache.CompletedTask;
        }

	    public override async Task ModelToDynamicAsync(RegisterWholesaleAccountModel model, CustomerDynamic dynamic)
	    {
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();

            var profileAddress = await _addressMapper.FromModelAsync(model, (int)AddressType.Profile);
				
			dynamic.ProfileAddress = profileAddress;

		    var shippingAddress = await _addressMapper.FromModelAsync(model, (int)AddressType.Shipping);
		    shippingAddress.Data.Default = true;
			dynamic.ShippingAddresses.Add(shippingAddress);
            
            dynamic.Data.TradeClass = model.TradeClass;
            dynamic.Data.PromotingWebsites = model.PromotingWebsites;
        }
	}
}
