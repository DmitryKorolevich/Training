using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Product;
using VitalChoice.Interfaces.Services.Product;

namespace VitalChoice.Business.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly IEcommerceRepositoryAsync<VProductSku> vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> lookupRepository;
        private readonly ILogger logger;

        public ProductService(IEcommerceRepositoryAsync<VProductSku> vProductSkuRepository, IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository)
        {
            this.vProductSkuRepository = vProductSkuRepository;
            this.productOptionTypeRepository = productOptionTypeRepository;
            this.lookupRepository = lookupRepository;
            logger = LoggerService.GetDefault();
        }

        public async Task<ICollection<ProductOptionType>> GetProductLookupsAsync()
        {
            ICollection<ProductOptionType> toReturn = (await productOptionTypeRepository.Query(p => p.IdLookup.HasValue).SelectAsync()).ToList();
            var lookups = (await lookupRepository.Query(p => toReturn.Select(pp=>pp.IdLookup.Value).Contains(p.Id)).Include(p=>p.LookupVariants).SelectAsync()).ToList();
            foreach(var item in toReturn)
            {
                foreach(var lookup in lookups)
                {
                    if(item.IdLookup==lookup.Id)
                    {
                        item.Lookup = lookup;
                    }
                }
            }

            return toReturn;
        }

        public async Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter)
        {
            var query = vProductSkuRepository.Query();

            var conditions = new VProductSkuConditions();
            conditions.Init(query);
            conditions = conditions.NotDeleted().WithText(filter.SearchText);

            Func<IQueryable<VProductSku>, IOrderedQueryable<VProductSku>> sortable = x => x.OrderByDescending(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VProductSkuSortPath.Name:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
            }

            PagedList<VProductSku> toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            return toReturn;
        }
    }
}