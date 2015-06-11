using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;

namespace VitalChoice.Data.Repositories.Customs
{
    public class VProductSkuRepository : EcommerceRepositoryAsync<VProductSku>
    {
        public VProductSkuRepository(IDataContextAsync context) : base(context)
		{
        }

        public async Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter)
        {
            var query = this.DbSet.Where(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);
            if(!String.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(x => x.Code.Contains(filter.SearchText) || x.Name.Contains(filter.SearchText));
            }

            Func<IQueryable<VProductSku>, IOrderedQueryable<VProductSku>> sortable = x => x.OrderByDescending(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VProductSkuSortPath.Name:
                    sortable =
                        x =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
            }
            query = sortable(query);
            var groups = query.GroupBy(p=>p.IdProduct).Select(g => new { IdProduct = g.Key, Name = g.Min(p=>p.Name) });
            var count = await groups.CountAsync();
            groups = groups.Skip((filter.Paging.PageIndex - 1) * filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount);
            var items = await groups.ToListAsync();

            return await Task.FromResult<PagedList<VProductSku>>(null);
        }
    }
}
