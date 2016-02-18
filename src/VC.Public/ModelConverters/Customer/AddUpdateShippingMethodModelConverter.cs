using System;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class AddUpdateShippingMethodModelConverter : BaseModelConverter<AddUpdateShippingMethodModel, AddressDynamic>
	{
		public override void DynamicToModel(AddUpdateShippingMethodModel model, AddressDynamic dynamic)
		{

		}

		public override void ModelToDynamic(AddUpdateShippingMethodModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
        }
	}
}
