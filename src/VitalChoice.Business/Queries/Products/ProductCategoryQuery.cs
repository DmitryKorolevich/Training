using System.Collections.Generic;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

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

        public ProductCategoryQuery WithStatus(ICollection<RecordStatusCode> statuses)
        {
            if (statuses != null && statuses.Count > 0)
            {
                Add(x => statuses.Contains(x.StatusCode));
            }

            return this;
        }
    }
}