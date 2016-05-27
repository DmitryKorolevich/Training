using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class CreditCardModelConverter : BaseModelConverter<CreditCardModel, CustomerPaymentMethodDynamic>
    {
        public override Task DynamicToModelAsync(CreditCardModel model, CustomerPaymentMethodDynamic dynamic)
        {
            if (dynamic.DictionaryData.ContainsKey("ExpDate"))
            {
                DateTime exp = dynamic.Data.ExpDate;
                model.ExpirationDateMonth = exp.Month;
                model.ExpirationDateYear = exp.Year%2000;
            }
            model.IdCustomerPaymentMethod = model.Id;
            return TaskCache.CompletedTask;
        }

        public override Task ModelToDynamicAsync(CreditCardModel model, CustomerPaymentMethodDynamic dynamic)
        {
            if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
            {
                DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
                dynamic.Data.ExpDate = exp;
            }
            return TaskCache.CompletedTask;
        }
    }
}