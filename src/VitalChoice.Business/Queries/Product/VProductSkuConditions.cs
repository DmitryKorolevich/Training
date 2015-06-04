using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Entities.Product;

namespace VitalChoice.Business.Queries.Product
{
    public class VProductSkuConditions : SimpleConditions<VProductSku>
    {
        public VProductSkuConditions WithText(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                _queryFluent.Where(x => x.Code.Contains(text) || x.Name.Contains(text));
            }

            return this;
        }

        public VProductSkuConditions NotDeleted()
        {
            _queryFluent.Where(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public VProductSkuConditions WithType(ProductType? type)
        {
            if (type.HasValue)
            {
                _queryFluent.Where(x => x.IdProductType == type);
            }

            return this;
        }
    }
}