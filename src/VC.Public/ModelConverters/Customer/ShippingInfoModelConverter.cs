using System;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class ShippingInfoModelConverter : BaseModelConverter<ShippingInfoModel, AddressDynamic>
	{
		public override void DynamicToModel(ShippingInfoModel model, AddressDynamic dynamic)
		{

		}

		public override void ModelToDynamic(ShippingInfoModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
        }
	}
}
