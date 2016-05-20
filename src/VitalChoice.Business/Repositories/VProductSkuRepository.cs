using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.Repositories
{
    public class VProductSkuRepository : EcommerceRepositoryAsync<VProductSku>
    {
        public VProductSkuRepository(EcommerceContext context) : base(context)
		{
        }

        public async Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter)
        {
            var query = this.DbSet.Where(x => (x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive));
            if(!String.IsNullOrEmpty(filter.SearchText))
            {
                query = query.Where(x => x.Code.Contains(filter.SearchText) || x.Name.Contains(filter.SearchText));
            }
	        if (filter.IdProducts != null && filter.IdProducts.Any())
	        {
				query = query.Where(x => filter.IdProducts.Contains(x.IdProduct));
	        }

	        Func<IQueryable<VProductSku>, IOrderedQueryable<VProductSku>> sortable = x => x.OrderByDescending(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VProductSkuSortPath.Name:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
                case VProductSkuSortPath.DateEdited:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VProductSkuSortPath.TaxCode:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.TaxCode)
                                : x.OrderByDescending(y => y.TaxCode);
                    break;
                case VProductSkuSortPath.IdProductType:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.IdProductType)
                                : x.OrderByDescending(y => y.IdProductType);
                    break;
            }

            query = query.GroupBy(p => p.IdProduct).Select(g => new VProductSku()
            {
                IdProduct = g.Key,
                Name = g.Min(p => p.Name),
                SubTitle = g.Min(p => p.SubTitle),
                Thumbnail = g.Min(p => p.Thumbnail),
                TaxCode = g.Min(p => p.TaxCode),
                StatusCode = g.Min(p => p.StatusCode),
                Hidden = g.Min(p => p.Hidden),
                DateEdited= g.Min(p => p.DateEdited),
                IdEditedBy = g.Min(p => p.IdEditedBy),
                IdProductType = g.Min(p => p.IdProductType)
            });
            var count = await query.CountAsync();
            query = sortable(query);
            if (filter.Paging != null)
            {
                query = query.Skip((filter.Paging.PageIndex - 1) * filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount);
            }
            var items = await query.ToListAsync();
            var toReturn = new PagedList<VProductSku>(items, count);

            return toReturn;
        }
    }
}
