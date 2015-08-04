using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Product
{
    public class VProductsWithReviewQuery : QueryObject<VProductsWithReview>
    {
        public VProductsWithReviewQuery WithName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Add(x => x.ProductName.Contains(name));
            }
            return this;
        }

        public VProductsWithReviewQuery WithStatus(RecordStatusCode status)
        {
            Add(x => x.StatusCode == status);
            return this;
        }
    }
}