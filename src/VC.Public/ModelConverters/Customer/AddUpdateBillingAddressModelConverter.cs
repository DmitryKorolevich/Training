using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain;

namespace VC.Public.ModelConverters.Customer
{
	public class AddUpdateBillingAddressModelConverter : BaseModelConverter<AddUpdateBillingAddressModel, AddressDynamic>
	{
		public override Task DynamicToModelAsync(AddUpdateBillingAddressModel model, AddressDynamic dynamic)
		{
            return TaskCache.CompletedTask;
        }

		public override Task ModelToDynamicAsync(AddUpdateBillingAddressModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
            return TaskCache.CompletedTask;
        }
	}
}
