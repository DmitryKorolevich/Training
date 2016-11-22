using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.Products
{
    public class SkuOOSHistoryItemQuery : QueryObject<SkuOOSHistoryItem>
    {
        public SkuOOSHistoryItemQuery ByDates(DateTime from, DateTime to)
        {
            Add(s => s.StartDate<=from && (!s.EndDate.HasValue || s.EndDate.Value>=to));
            return this;
        }

        public SkuOOSHistoryItemQuery BySkuIds(ICollection<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                Add(s => ids.Contains(s.Id));
            }
            return this;
        }
    }
}