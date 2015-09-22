﻿using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Product
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

        public DiscountQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);
            return this;
        }

        public DiscountQuery WithStatus(RecordStatusCode? status)
        {
            if (status.HasValue)
            {
                Add(x => x.StatusCode == status.Value);
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
                var from = new DateTime(validFrom.Value.Year, validFrom.Value.Month, validFrom.Value.Day, 0, 0, 0);
                Add(x => !x.StartDate.HasValue || x.StartDate.Value>= from);
            }
            return this;
        }

        public DiscountQuery WithValidTo(DateTime? validTo)
        {
            if (validTo.HasValue)
            {
                var to = (new DateTime(validTo.Value.Year, validTo.Value.Month, validTo.Value.Day, 0, 0, 0)).AddDays(1);
                Add(x => !x.ExpirationDate.HasValue || x.ExpirationDate.Value <= to);
            }
            return this;
        }
    }
}