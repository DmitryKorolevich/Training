using System;
using System.Threading.Tasks;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class AddUpdateBillingAddressModelConverter : BaseModelConverter<AddUpdateBillingAddressModel, AddressDynamic>
	{
		public override Task DynamicToModelAsync(AddUpdateBillingAddressModel model, AddressDynamic dynamic)
		{
            return Task.Delay(0);
        }

		public override Task ModelToDynamicAsync(AddUpdateBillingAddressModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
            return Task.Delay(0);
        }
	}
}
