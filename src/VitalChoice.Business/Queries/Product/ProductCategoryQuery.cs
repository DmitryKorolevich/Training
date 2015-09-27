using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductCategoryQuery : QueryObject<ProductCategory>
    {
        public ProductCategoryQuery WithId(int id)
        {
            Add(x => x.Id == id);

            return this;
        }

        public ProductCategoryQuery WithParentId(int id)
        {
            Add(x => x.ParentId == id);

            return this;
        }

        public ProductCategoryQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public ProductCategoryQuery WithStatus(IList<RecordStatusCode> statuses)
        {
            if (statuses != null && statuses.Any())
            {
                Add(x => statuses.Contains(x.StatusCode));
            }

            return this;
        }
    }
}