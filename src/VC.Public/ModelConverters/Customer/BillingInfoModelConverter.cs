using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Profile;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Public.ModelConverters.Customer
{
	public class BillingInfoModelConverter : IModelToDynamicConverter<BillingInfoModel, CustomerPaymentMethodDynamic>
	{
		public BillingInfoModelConverter()
		{
		}

		public void DynamicToModel(BillingInfoModel model, CustomerPaymentMethodDynamic dynamic)
		{
			if (dynamic.DictionaryData.ContainsKey("ExpDate"))
			{
				DateTime exp = dynamic.Data.ExpDate;
				model.ExpirationDateMonth = exp.Month;
				model.ExpirationDateYear = exp.Year % 2000;
			}
		}

		public void ModelToDynamic(BillingInfoModel model, CustomerPaymentMethodDynamic dynamic)
		{
			if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
			{
				DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
				dynamic.Data.ExpDate = exp;
			}
		}
	}
}
