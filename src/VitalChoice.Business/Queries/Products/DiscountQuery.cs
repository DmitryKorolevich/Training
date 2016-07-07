using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Queries.Products
{
    public class DiscountQuery : QueryObject<Discount>
    {
        public DiscountQuery WithText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Add(x => x.Code.Contains(text) || x.Description.Contains(text));
            }
            return this;
        }

        public DiscountQuery WithCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Add(x => x.Code.Contains(code));
            }
            return this;
        }

        public DiscountQuery WithEqualCode(string code)
        {
            Add(x => x.Code == code);
            return this;
        }

        public DiscountQuery NotDeleted()
        {
            Add(x => x.StatusCode == (int)RecordStatusCode.Active || x.StatusCode == (int)RecordStatusCode.NotActive);
            return this;
        }

        public DiscountQuery WithStatus(RecordStatusCode? status)
        {
            if (status.HasValue)
            {
                var statusInt = (int) status.Value;
                Add(x => x.StatusCode == statusInt);
            }
            return this;
        }

        public DiscountQuery WithAssigned(bool searchByAssigned, CustomerType? type)
        {
            if (searchByAssigned)
            {
                if (type.HasValue)
                {
                    Add(x => x.Assigned != null && x.Assigned.Value == type.Value);
            }
                else
                {
                    Add(x => x.Assigned == null);
                }
            }
            return this;
        }

        public DiscountQuery WithType(DiscountType? type)
        {
            if (type.HasValue)
            {
                var typeInt = (int) type;
                Add(x => x.IdObjectType == typeInt);
            }

            return this;
        }

        public DiscountQuery WithValidFrom(DateTime? validFrom)
        {
            if (validFrom.HasValue)
            {
                var from = validFrom;
                Add(x => x.StartDate >= from);
            }
            return this;
        }

        public DiscountQuery WithValidTo(DateTime? validTo)
        {
            if (validTo.HasValue)
            {
                var to = validTo;
                Add(x => x.ExpirationDate == null || x.ExpirationDate.Value <= to);
            }
            return this;
        }

        public DiscountQuery WithExpiredType(ExpiredType? expiredType)
        {
            if (expiredType.HasValue)
            {
                DateTime endNow = DateTime.Now.AddDays(-1);//Discount is valid the entire end date
                if (expiredType.Value==ExpiredType.Expired)
                {
                    Add(x => x.ExpirationDate != null && x.ExpirationDate.Value <= endNow);
                }
                if (expiredType.Value == ExpiredType.NotExpired)
                {
                    Add(x => x.ExpirationDate != null && x.ExpirationDate.Value > endNow);
                }
            }
            return this;
        }

        public DiscountQuery WithDateStatus(DateStatus? dateStatus)
        {
            if (dateStatus.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime dayBeforeNow = now.AddDays(-1);//Discount is valid the entire end date
                if (dateStatus.Value == DateStatus.Expired)
                {
                    Add(x => x.ExpirationDate != null && x.ExpirationDate.Value <= dayBeforeNow);
                }
                if (dateStatus.Value == DateStatus.Live)
                {
                    Add(x => x.ExpirationDate != null && x.ExpirationDate.Value > dayBeforeNow && now >= x.StartDate);
                }
                if (dateStatus.Value == DateStatus.Future)
                {
                    Add(x => now < x.StartDate);
                }
            }
            return this;
        }
    }
}