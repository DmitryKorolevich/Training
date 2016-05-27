using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class CusomerAddressModelConverter : BaseModelConverter<AddressModel, AddressDynamic>
    {
	    public override Task DynamicToModelAsync(AddressModel model, AddressDynamic dynamic)
	    {
		    model.Country.Id = dynamic.IdCountry;
	        if (!model.PreferredShipMethod.HasValue && dynamic.IdObjectType == (int) AddressType.Shipping)
	        {
	            model.PreferredShipMethod=PreferredShipMethod.Best;
	        }
            if (!model.ShippingAddressType.HasValue && dynamic.IdObjectType == (int)AddressType.Shipping)
            {
                model.ShippingAddressType = ShippingAddressType.Residential;
            }
	        return TaskCache.CompletedTask;
	    }

	    public override Task ModelToDynamicAsync(AddressModel model, AddressDynamic dynamic)
	    {
		    dynamic.IdCountry = model.Country.Id;
	        if (model.Country.States == null || !model.Country.States.Any())
	        {
	            dynamic.IdState = null;
	        }
            return TaskCache.CompletedTask;
        }
	}
}
