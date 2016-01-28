﻿using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
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
                Add(x => x.StatusCode == (int)status.Value);
            }
            return this;
        }

        public DiscountQuery WithType(DiscountType? type)
        {
            if (type.HasValue)
            {
                Add(x => x.IdObjectType == (int?)type);
            }

            return this;
        }

        public DiscountQuery WithValidFrom(DateTime? validFrom)
        {
            if (validFrom.HasValue)
            {
                var from = validFrom;
                Add(x => !x.StartDate.HasValue || x.StartDate.Value>= from);
            }
            return this;
        }

        public DiscountQuery WithValidTo(DateTime? validTo)
        {
            if (validTo.HasValue)
            {
                var to = validTo;
                Add(x => !x.ExpirationDate.HasValue || x.ExpirationDate.Value <= to);
            }
            return this;
        }

        public DiscountQuery WithExpiredType(ExpiredType? expiredType)
        {
            if (expiredType.HasValue)
            {
                DateTime date = DateTime.Now.AddDays(-1);//Discount is valid the entire end date
                if (expiredType.Value==ExpiredType.Expired)
                {
                    Add(x => x.ExpirationDate<= date);
                }
                if (expiredType.Value == ExpiredType.NotExpired)
                {
                    Add(x => x.ExpirationDate > date);
                }
            }
            return this;
        }

        public DiscountQuery WithDateStatus(DateStatus? dateStatus)
        {
            if (dateStatus.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime dateBefore = now.AddDays(-1);//Discount is valid the entire end date
                if (dateStatus.Value == DateStatus.Expired)
                {
                    Add(x => x.ExpirationDate <= dateBefore);
                }
                if (dateStatus.Value == DateStatus.Live)
                {
                    Add(x => x.ExpirationDate > dateBefore && now>=x.StartDate);
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