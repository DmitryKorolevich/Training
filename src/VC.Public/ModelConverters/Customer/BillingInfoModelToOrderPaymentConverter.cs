﻿using System;
using VC.Public.Models.Profile;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Customer
{
    public class BillingInfoModelToOrderPaymentConverter : BaseModelConverter<BillingInfoModel, OrderPaymentMethodDynamic>
    {
        public override void DynamicToModel(BillingInfoModel model, OrderPaymentMethodDynamic dynamic)
        {
            if (dynamic.SafeData.ExpDate != null)
            {
                DateTime exp = dynamic.Data.ExpDate;
                model.ExpirationDateMonth = exp.Month;
                model.ExpirationDateYear = exp.Year % 2000;
            }
        }

        public override void ModelToDynamic(BillingInfoModel model, OrderPaymentMethodDynamic dynamic)
        {
            if (model.ExpirationDateYear.HasValue && model.ExpirationDateMonth.HasValue)
            {
                DateTime exp = new DateTime(model.ExpirationDateYear.Value + 2000, model.ExpirationDateMonth.Value, 1);
                dynamic.Data.ExpDate = exp;
                if (exp.AddMonths(1).AddDays(-1) < DateTime.Today)
                    throw new AppValidationException(string.Empty, "Your credit card is expired.");
            }
        }
    }
}