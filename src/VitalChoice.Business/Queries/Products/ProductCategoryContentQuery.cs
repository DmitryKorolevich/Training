using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductCategoryContentQuery : QueryObject<ProductCategoryContent>
    {
        public ProductCategoryContentQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public ProductCategoryContentQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode == status);
            }

            return this;
        }

		public ProductCategoryContentQuery WithVisibility(IList<CustomerTypeCode> codes, bool showAll=false)
		{
            if (!showAll)
            {
                if (codes != null)
                {
                    Add(x => x.NavIdVisible.HasValue && codes.Contains(x.NavIdVisible.Value));
                }
                else
                {
                    Add(x => x.NavIdVisible.HasValue);
                }
            }

			return this;
		}
	}
}