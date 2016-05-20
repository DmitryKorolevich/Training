using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Profile;
using VitalChoice.Business.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
    public class BillingInfoModelToOrderPaymentConverter : BaseModelConverter<BillingInfoModel, OrderPaymentMethodDynamic>
    {
        public override Task DynamicToModelAsync(BillingInfoModel model, OrderPaymentMethodDynamic dynamic)
        {
            if (dynamic.SafeData.ExpDate != null)
            {
                DateTime exp = dynamic.Data.ExpDate;
                model.ExpirationDateMonth = exp.Month;
                model.ExpirationDateYear = exp.Year % 2000;
            }
            return TaskCache.CompletedTask;
        }

        public override Task ModelToDynamicAsync(BillingInfoModel model, OrderPaymentMethodDynamic dynamic)
        {
            dynamic.Data.Phone = model.Phone?.ClearPhone();
            dynamic.Data.Fax = model.Fax?.ClearPhone();

            if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
            {
                DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
                dynamic.Data.ExpDate = exp;
                if (exp.AddMonths(1).AddDays(-1) < DateTime.Today)
                    throw new AppValidationException(string.Empty, "Your credit card is expired.");
            }
            return TaskCache.CompletedTask;
        }
    }
}