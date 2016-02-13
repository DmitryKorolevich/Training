using System;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class AddUpdateBillingAddressModelConverter : BaseModelConverter<AddUpdateBillingAddressModel, AddressDynamic>
	{
		public override void DynamicToModel(AddUpdateBillingAddressModel model, AddressDynamic dynamic)
		{

		}

		public override void ModelToDynamic(AddUpdateBillingAddressModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
        }
	}
}
