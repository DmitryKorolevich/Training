using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class SkuQuery : QueryObject<Sku>
    {
        public SkuQuery NotDeleted()
        {
            Add(s => s.StatusCode != RecordStatusCode.Deleted);
            return this;
        }

        public SkuQuery Excluding(ICollection<int> ids)
        {
            if (ids != null && ids.Any())
                Add(s => !ids.Contains(s.Id));
            return this;
        }

        public SkuQuery Including(ICollection<string> codes)
        {
            Add(s => codes.Contains(s.Code));
            return this;
        }

        public SkuQuery WithProductId(int id)
        {
            Add(s => s.IdProduct == id);
            return this;
        }
    }
}