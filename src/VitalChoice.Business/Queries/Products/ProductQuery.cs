using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductQuery : QueryObject<Ecommerce.Domain.Entities.Products.Product>
    {
        public ProductQuery Excluding(int? id)
        {
            if (id.HasValue && id.Value > 0)
                Add(p => p.Id != id.Value);
            return this;
        }

        public ProductQuery NotDeleted()
        {
            Add(p => p.StatusCode != (int)RecordStatusCode.Deleted);
            return this;
        }

        public ProductQuery WithName(string name)
        {
            Add(p => p.Name == name);
            return this;
        }

		public ProductQuery WithPublicId(Guid publicId)
		{
			Add(p => p.PublicId == publicId);
			return this;
		}
	}
}
