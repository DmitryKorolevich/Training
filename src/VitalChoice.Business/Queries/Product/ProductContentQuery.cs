using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductContentQuery : QueryObject<ProductContent>
    {
        public ProductContentQuery Excluding(int? id)
        {
            if (id.HasValue && id.Value > 0)
                Add(p => p.Id != id.Value);
            return this;
        }

        public ProductContentQuery NotDeleted()
        {
            Add(p => p.StatusCode != RecordStatusCode.Deleted);
            return this;
        }

        public ProductContentQuery WithUrl(string url)
        {
            Add(p => p.Url == url);
            return this;
        }
    }
}
