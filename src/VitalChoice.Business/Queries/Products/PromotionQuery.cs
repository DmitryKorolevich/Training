using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Queries.Product
{
    public class PromotionQuery : QueryObject<Promotion>
    {
        public PromotionQuery WithDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                Add(x => x.Description.Contains(description));
            }
            return this;
        }

        public PromotionQuery IsActive()
        {
            Add(x => x.StatusCode == (int)RecordStatusCode.Active);
            return this;
        }

        public PromotionQuery NotDeleted()
        {
            Add(x => x.StatusCode == (int)RecordStatusCode.Active || x.StatusCode == (int)RecordStatusCode.NotActive);
            return this;
        }

        public PromotionQuery WithStatus(RecordStatusCode? status)
        {
            if (status.HasValue)
            {
                Add(x => x.StatusCode == (int)status.Value);
            }
            return this;
        }

        public PromotionQuery WithType(PromotionType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.IdObjectType == (int?)type);
            }

            return this;
        }

        public PromotionQuery WithValidFrom(DateTime? validFrom)
        {
            if (validFrom.HasValue)
            {
                var from = validFrom;
                Add(x => !x.StartDate.HasValue || x.StartDate.Value >= from);
            }
            return this;
        }

        public PromotionQuery WithValidTo(DateTime? validTo)
        {
            if (validTo.HasValue)
            {
                var to = validTo;
                Add(x => !x.ExpirationDate.HasValue || x.ExpirationDate.Value <= to);
            }
            return this;
        }

        public PromotionQuery WithExpiredType(ExpiredType? expiredType)
        {
            if (expiredType.HasValue)
            {
                DateTime date = DateTime.Now.AddDays(-1);//Promotion is valid the entire end date
                if (expiredType.Value == ExpiredType.Expired)
                {
                    Add(x => x.ExpirationDate <= date);
                }
                if (expiredType.Value == ExpiredType.NotExpired)
                {
                    Add(x => x.ExpirationDate > date);
                }
            }
            return this;
        }

        public PromotionQuery AllowCustomerType(CustomerType customerType)
        {
            Add(p => p.Assigned == null || p.Assigned == customerType);
            return this;
        }
    }
}