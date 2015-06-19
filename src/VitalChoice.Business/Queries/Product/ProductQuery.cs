using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductQuery : QueryObject<Domain.Entities.eCommerce.Products.Product>
    {
        public ProductQuery Excluding(int? id)
        {
            if (id.HasValue)
                Add(p => p.Id != id.Value);
            return this;
        }

        public ProductQuery NotDeleted()
        {
            Add(p => p.StatusCode != RecordStatusCode.Deleted);
            return this;
        }

        public ProductQuery WithName(string name)
        {
            Add(p => p.Name == name);
            return this;
        }
    }
}
