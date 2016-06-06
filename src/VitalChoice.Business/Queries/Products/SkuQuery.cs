using System.Collections.Generic;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class SkuQuery : QueryObject<Sku>
    {
        public SkuQuery NotDeleted()
        {
            Add(s => s.StatusCode != (int)RecordStatusCode.Deleted);
            return this;
        }

        public SkuQuery Excluding(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
                Add(s => !ids.Contains(s.Id));
            return this;
        }

        public SkuQuery Including(ICollection<string> codes)
        {
            Add(s => codes.Contains(s.Code));
            return this;
        }

        public SkuQuery ByIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(s => ids.Contains(s.Id));
            }
            return this;
        }

        public SkuQuery ByProductIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(s => ids.Contains(s.IdProduct));
            }
            return this;
        }

        public SkuQuery WithProductId(int id)
        {
            Add(s => s.IdProduct == id);
            return this;
        }

        public SkuQuery WithCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(s => s.Code.Contains(code));
            }
            return this;
        }
    }
}