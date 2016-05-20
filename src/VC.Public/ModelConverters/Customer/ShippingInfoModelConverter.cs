using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class ShippingInfoModelConverter : BaseModelConverter<ShippingInfoModel, AddressDynamic>
	{
		public override Task DynamicToModelAsync(ShippingInfoModel model, AddressDynamic dynamic)
		{
			if (dynamic.SafeData.PreferredShipMethod == null && dynamic.IdObjectType == (int)AddressType.Shipping)
			{
				model.PreferredShipMethod = PreferredShipMethod.Best;
			}
			if (dynamic.SafeData.ShippingAddressType == null && dynamic.IdObjectType == (int)AddressType.Shipping)
			{
				model.AddressType = ShippingAddressType.Residential;
			}
            return TaskCache.CompletedTask;
        }

		public override Task ModelToDynamicAsync(ShippingInfoModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
            return TaskCache.CompletedTask;

            //if (!model.PreferredShipMethod.HasValue && dynamic.IdObjectType == (int)AddressType.Shipping)
            //{
            //	dynamic.Data.PreferredShipMethod = PreferredShipMethod.Best;
            //}
            //if (!model.AddressType.HasValue && dynamic.IdObjectType == (int)AddressType.Shipping)
            //{
            //	dynamic.Data.ShippingAddressType = ShippingAddressType.Residential;
            //}
        }
	}
}
