using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.Queries.Orders
{
    public class VOrderWithRegionInfoItemQuery : QueryObject<VOrderWithRegionInfoItem>
    {
        public VOrderWithRegionInfoItemQuery WithIdCustomerType(int? idCustomerType)
        {
            if (idCustomerType.HasValue)
            {
                Add(x => x.IdCustomerType == idCustomerType.Value);
            }
            return this;
        }

        public VOrderWithRegionInfoItemQuery WithIdOrderType(int? idOrderType)
        {
            if (idOrderType.HasValue)
            {
                Add(x => x.OrderType == Convert.ToString(idOrderType.Value));
            }
            return this;
        }

        public VOrderWithRegionInfoItemQuery WithDates(DateTime from, DateTime to)
        {
            Add(x => x.DateCreated >= from && x.DateCreated < to);
            return this;
        }

        public VOrderWithRegionInfoItemQuery WithRegion(string region)
        {
            if (!String.IsNullOrEmpty(region))
            {
                Add(x => x.Region == region);
            }
            return this;
        }

        public VOrderWithRegionInfoItemQuery WithZip(string zip)
        {
            if (!String.IsNullOrEmpty(zip))
            {
                Add(x => x.Zip == zip);
            }
            return this;
        }
    }
}