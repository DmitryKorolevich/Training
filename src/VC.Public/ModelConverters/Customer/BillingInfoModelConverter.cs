using System;
using VC.Public.Models.Profile;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
	public class BillingInfoModelConverter : BaseModelConverter<BillingInfoModel, CustomerPaymentMethodDynamic>
	{
		public BillingInfoModelConverter()
		{
		}

		public override void DynamicToModel(BillingInfoModel model, CustomerPaymentMethodDynamic dynamic)
		{
			if (dynamic.DictionaryData.ContainsKey("ExpDate"))
			{
				DateTime exp = dynamic.Data.ExpDate;
				model.ExpirationDateMonth = exp.Month;
				model.ExpirationDateYear = exp.Year % 2000;
			}
		}

		public override void ModelToDynamic(BillingInfoModel model, CustomerPaymentMethodDynamic dynamic)
		{
			if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
			{
				DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
				dynamic.Data.ExpDate = exp;
			}
		}
	}
}
