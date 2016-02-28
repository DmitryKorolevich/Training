using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class VSkuQuery : QueryObject<VSku>
    {
        public VSkuQuery WithText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Add(x => x.Code.Contains(text) || x.Name.Contains(text));
            }
            return this;
        }

        public VSkuQuery WithExactDescriptionName(string descriptionName)
        {
            if (!string.IsNullOrEmpty(descriptionName))
            {
                Add(x => x.DescriptionName == descriptionName);
            }
            return this;
        }

        public VSkuQuery WithExactCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(x => x.Code == code);
            }
            return this;
        }

        public VSkuQuery WithExactCodes(ICollection<string> codes)
        {
            if (codes != null)
            {
                Add(x => codes.Contains(x.Code));
            }
            return this;
        }

        public VSkuQuery WithDescriptionName(string descriptionName)
        {
            if (!string.IsNullOrEmpty(descriptionName))
            {
                Add(x => x.DescriptionName.Contains(descriptionName));
            }
            return this;
        }

        public VSkuQuery WithCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(x => x.Code.Contains(code));
            }
            return this;
        }

        public VSkuQuery NotDeleted()
        {
            Add(x => (x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive) &&
                (x.SkuStatusCode == RecordStatusCode.Active || x.SkuStatusCode == RecordStatusCode.NotActive));
            return this;
        }

        public VSkuQuery WithActiveSku()
        {
            Add(x => x.SkuStatusCode == RecordStatusCode.Active);
            return this;
        }

        public VSkuQuery WithActiveProduct()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active);
            return this;
        }

        public VSkuQuery WithType(ProductType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.IdProductType == type);
            }

            return this;
        }

        public VSkuQuery WithIds(IList<int> ids)
        {
            if (ids!=null && ids.Count > 0)
            {
                Add(x => ids.Contains(x.SkuId));
            }

            return this;
        }

        public VSkuQuery WithIdProducts(IList<int> idProducts)
        {
            if (idProducts!=null && idProducts.Count>0)
            {
                Add(x => idProducts.Contains(x.IdProduct));
            }

            return this;
        }

	    public VSkuQuery NotHiddenOnly(bool notHidden)
	    {
			if (notHidden)
			{
				Add(x => !x.Hidden && !x.ProductHidden);
			}

			return this;
		}

		public VSkuQuery ActiveOnly(bool active)
		{
			if (active)
			{
				Add(x => x.StatusCode == RecordStatusCode.Active && x.SkuStatusCode == RecordStatusCode.Active);
			}

			return this;
		}
	}
}