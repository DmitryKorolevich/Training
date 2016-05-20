using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class OrderCreditCardModelConverter : BaseModelConverter<CreditCardModel, OrderPaymentMethodDynamic>
    {
        public override Task DynamicToModelAsync(CreditCardModel model, OrderPaymentMethodDynamic dynamic)
        {
            if (dynamic.DictionaryData.ContainsKey("ExpDate"))
            {
                DateTime? exp = dynamic.SafeData.ExpDate;
                model.ExpirationDateMonth = exp?.Month;
                model.ExpirationDateYear = exp?.Year%2000;
            }
            return TaskCache.CompletedTask;
        }

        public override Task ModelToDynamicAsync(CreditCardModel model, OrderPaymentMethodDynamic dynamic)
        {
            if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
            {
                DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
                dynamic.Data.ExpDate = exp;
            }
            dynamic.IdCustomerPaymentMethod = model.IdCustomerPaymentMethod;
            return TaskCache.CompletedTask;
        }
    }
}