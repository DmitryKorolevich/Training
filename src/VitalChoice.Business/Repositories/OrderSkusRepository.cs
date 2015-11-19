using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Data.Repositories.Customs
{
    public class OrderSkusRepository : EcommerceRepositoryAsync<OrderToSku>
    {
        public OrderSkusRepository(IDataContextAsync context) : base(context)
		{
        }

        public async Task<Dictionary<int,int>> GetTopPurchasedSkuIdsAsync(FilterBase filter)
        {
            var query = this.DbSet.GroupBy(p => p.IdSku).Select(g => new
            {
                IdSku = g.Key,
                Count = g.Sum(p => p.Quantity),
            });

            query = query.OrderByDescending(p => p.Count);

            if (filter.Paging != null)
            {
                query = query.Skip((filter.Paging.PageIndex - 1) * filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount);
            }
            var items = await query.ToListAsync();
            var toReturn = items.ToDictionary(p => p.IdSku, x => x.Count);

            return toReturn;
        }
    }
}
