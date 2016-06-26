﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class AddUpdateShippingMethodModelConverter : BaseModelConverter<AddUpdateShippingMethodModel, AddressDynamic>
	{
		public override Task DynamicToModelAsync(AddUpdateShippingMethodModel model, AddressDynamic dynamic)
		{
            return TaskCache.CompletedTask;
        }

		public override Task ModelToDynamicAsync(AddUpdateShippingMethodModel model, AddressDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();
            return TaskCache.CompletedTask;
        }
	}
}
