using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

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

		public ProductCategoryContentQuery WithVisibility(IList<CustomerTypeCode> codes)
		{
			Add(x => x.NavIdVisible.HasValue && codes.Contains(x.NavIdVisible.Value));

			return this;
		}
	}
}