using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Models.Customer;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class CreditCardModelConverter : IModelToDynamicConverter<CreditCardModel, CustomerPaymentMethodDynamic>
    {
        public void DynamicToModel(CreditCardModel model, CustomerPaymentMethodDynamic dynamic)
        {
            DateTime exp = dynamic.Data.ExpDate;
            model.ExpirationDateMonth = exp.Month;
            model.ExpirationDateYear = exp.Year % 2000;
        }

        public void ModelToDynamic(CreditCardModel model, CustomerPaymentMethodDynamic dynamic)
        {
            DateTime exp = new DateTime(model.ExpirationDateYear + 2000, model.ExpirationDateMonth, 1);
            dynamic.Data.ExpDate = exp;
        }
    }
}
