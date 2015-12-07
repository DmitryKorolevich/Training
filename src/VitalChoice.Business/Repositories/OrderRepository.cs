using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.Repositories
{
    public class OrderRepository : EcommerceRepositoryAsync<Order>
    {
        public OrderRepository(IDataContextAsync context) : base(context)
        {
        }

        public Task<List<CustomerOrderStatistic>> GetCustomerOrderStatistics(ICollection<int> ids)
        {
            var query = this.DbSet.Where(p => ids.Contains(p.IdCustomer) && p.StatusCode!=(int)RecordStatusCode.Deleted);
            
            return query.GroupBy(p => p.IdCustomer).Select(g => new CustomerOrderStatistic()
            {
                IdCustomer = g.Key,
                TotalOrders = g.Count(),
                FirstOrderPlaced = g.Min(p => p.DateCreated),
                LastOrderPlaced = g.Max(p => p.DateCreated),
            }).ToListAsync();
        }

        public async Task<decimal> GetOrderWithRegionInfoAmountAsync(OrderRegionFilter filter)
        {
            VOrderWithRegionInfoItemQuery conditions = new VOrderWithRegionInfoItemQuery().WithDates(filter.From, filter.To).
                WithIdCustomerType(filter.IdCustomerType).WithIdOrderType(filter.IdOrderType).WithRegion(filter.Region).WithZip(filter.Zip);

            var toReturn = await (this.Context as DbContext).Set<VOrderWithRegionInfoItem>().Where(conditions.Query()).SumAsync(p => p.Total);
            return toReturn;
        }
    }
}
