using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class BillingInfoModelConverter : BaseModelConverter<BillingInfoModel, CustomerPaymentMethodDynamic>
	{
		public override Task DynamicToModelAsync(BillingInfoModel model, CustomerPaymentMethodDynamic dynamic)
		{
			if (dynamic.DictionaryData.ContainsKey("ExpDate"))
			{
				DateTime exp = dynamic.Data.ExpDate;
				model.ExpirationDateMonth = exp.Month;
				model.ExpirationDateYear = exp.Year % 2000;
			}
            return TaskCache.CompletedTask;
        }

		public override Task ModelToDynamicAsync(BillingInfoModel model, CustomerPaymentMethodDynamic dynamic)
		{
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();

            if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
			{
				DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
				dynamic.Data.ExpDate = exp;
			}
            return TaskCache.CompletedTask;
        }
	}
}
