using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Product
{
    public class VProductSkuQuery : QueryObject<VProductSku>
    {
        public VProductSkuQuery WithText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Add(x => x.Code.Contains(text) || x.Name.Contains(text));
            }
            return this;
        }

        public VProductSkuQuery NotDeleted()
        {
            Add(x => (x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive) &&
                (x.SkuStatusCode == RecordStatusCode.Active || x.SkuStatusCode == RecordStatusCode.NotActive));
            return this;
        }

        public VProductSkuQuery WithActiveSku()
        {
            Add(x => x.SkuStatusCode == RecordStatusCode.Active);
            return this;
        }

        public VProductSkuQuery WithActiveProduct()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active);
            return this;
        }

        public VProductSkuQuery WithType(ProductType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.IdProductType == type);
            }

            return this;
        }
    }
}