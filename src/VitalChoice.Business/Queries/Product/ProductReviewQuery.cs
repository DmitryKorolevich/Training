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
    public class ProductReviewQuery : QueryObject<ProductReview>
    {
        public ProductReviewQuery WithId(int id)
        {
            Add(x => x.Id.Equals(id));
            return this;
        }

        public ProductReviewQuery WithIdProduct(int? idProduct)
        {
            if (idProduct.HasValue)
            {
                Add(x => x.IdProduct== idProduct.Value);
            }
            return this;
        }

        public ProductReviewQuery NotDeleted()
        {
            Add(p => p.StatusCode != RecordStatusCode.Deleted);
            return this;
        }

        public ProductReviewQuery WithStatus(RecordStatusCode status)
        {
            Add(x => x.StatusCode == status);
            return this;
        }
    }
}