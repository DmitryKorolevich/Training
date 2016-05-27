using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Repositories
{
    public class OrderSkusRepository : EcommerceRepositoryAsync<VTopProducts>
    {
        public OrderSkusRepository(EcommerceContext context) : base(context)
		{
        }

        public async Task<Dictionary<int,int>> GetTopPurchasedSkuIdsAsync(FilterBase filter, int idCustomer)
        {
            var query = DbSet.Where(o => o.IdCustomer == idCustomer);

            query = query.OrderByDescending(p => p.Count);

            if (filter.Paging != null)
            {
                query = query.Skip((filter.Paging.PageIndex - 1) * filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount);
            }
            var toReturn = await query.ToDictionaryAsync(p => p.IdSku, x => x.Count);

            return toReturn;
        }
    }
}
