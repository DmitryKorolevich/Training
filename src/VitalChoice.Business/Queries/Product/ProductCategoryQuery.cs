using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductCategoryQuery : QueryObject<ProductCategory>
    {
        public ProductCategoryQuery WithId(int id)
        {
            Add(x => x.Id.Equals(id));

            return this;
        }

        public ProductCategoryQuery WithParentId(int id)
        {
            Add(x => x.ParentId.Equals(id));

            return this;
        }

        public ProductCategoryQuery NotDeleted()
        {
            Add(x => x.StatusCode.Equals(RecordStatusCode.Active) || x.StatusCode.Equals(RecordStatusCode.NotActive));

            return this;
        }

        public ProductCategoryQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode.Equals(status));
            }

            return this;
        }
    }
}