using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.Business.Queries.Order
{
    public class OrderOptionTypeQuery : OptionTypeQuery<OrderOptionType>
    {
        public override IQueryOptionType<OrderOptionType> WithObjectType(int? objectType)
        {
            Add(d => d.IdObjectType == objectType);
            return this;
        }
    }
}