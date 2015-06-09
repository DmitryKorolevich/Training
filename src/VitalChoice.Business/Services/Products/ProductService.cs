using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Interfaces.Services.Product;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IEcommerceRepositoryAsync<VProductSku> vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> lookupRepository;
        private readonly IEcommerceRepositoryAsync<Domain.Entities.eCommerce.Products.Product> productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> skuRepository;
        private readonly ILogger logger;

        public ProductService(IEcommerceRepositoryAsync<VProductSku> vProductSkuRepository, IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Domain.Entities.eCommerce.Products.Product> productRepository, IEcommerceRepositoryAsync<Sku> skuRepository)
        {
            this.vProductSkuRepository = vProductSkuRepository;
            this.productOptionTypeRepository = productOptionTypeRepository;
            this.lookupRepository = lookupRepository;
            this.productRepository = productRepository;
            this.skuRepository = skuRepository;
            logger = LoggerService.GetDefault();
        }

        #region ProductOptions

        public async Task<ICollection<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names)
        {
            ICollection<ProductOptionType> toReturn = (await productOptionTypeRepository.Query(p => names.Contains(p.Name)).SelectAsync()).ToList();
            return toReturn;
        }

        public async Task<Dictionary<int, Dictionary<string, string>>> GetProductEditDefaultSettingsAsync()
        {
            Dictionary<int, Dictionary<string, string>> toReturn = new Dictionary<int, Dictionary<string, string>>();
            List<string> names = new List<string>();
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_CROSS_SELL_PRODUCT; i++)
            {
                names.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i);
                names.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i);
            }
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_YOUTUBE; i++)
            {
                names.Add(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i);
                names.Add(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i);
                names.Add(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i);
            }
            var items = await GetProductOptionTypesAsync(names);
            foreach (var item in items.Where(p => p.IdProductType.HasValue))
            {
                Dictionary<string, string> productTypeDefaultValues = null;
                if (toReturn.ContainsKey((int)item.IdProductType.Value))
                {
                    productTypeDefaultValues = toReturn[(int)item.IdProductType.Value];
                }
                else
                {
                    productTypeDefaultValues = new Dictionary<string, string>();
                    toReturn.Add((int)item.IdProductType.Value, productTypeDefaultValues);
                }
                if (!productTypeDefaultValues.ContainsKey(item.Name))
                {
                    productTypeDefaultValues.Add(item.Name, item.DefaultValue);
                }
            }

            return toReturn;
        }

        public async Task<ICollection<ProductOptionType>> GetProductLookupsAsync()
        {
            ICollection<ProductOptionType> toReturn = (await productOptionTypeRepository.Query(p => p.IdLookup.HasValue).SelectAsync()).ToList();
            var lookups = (await lookupRepository.Query(p => toReturn.Select(pp => pp.IdLookup.Value).Contains(p.Id)).Include(p => p.LookupVariants).SelectAsync()).ToList();
            foreach (var lookup in lookups)
            {
                lookup.LookupVariants = lookup.LookupVariants.OrderBy(p => p.Id).ToList();
            }
            foreach (var item in toReturn)
            {
                foreach (var lookup in lookups)
                {
                    if (item.IdLookup == lookup.Id)
                    {
                        item.Lookup = lookup;
                    }
                }
            }

            return toReturn;
        }

        #endregion

        #region Products

        public async Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter)
        {
            var conditions = new VProductSkuQuery().NotDeleted().WithText(filter.SearchText);
            var query = vProductSkuRepository.Query(conditions);

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

            PagedList<VProductSku> toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            return toReturn;
        }

        public async Task<ProductDynamic> GetProductAsync(int id, bool withDefaults=false)
        {
            IQueryFluent<Product> res = productRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).Include(p => p.OptionValues)
                .Include(p=>p.ProductsToCategories).Include(p => p.OptionTypes);
            var item =(await res.SelectAsync(false)).FirstOrDefault();
            ProductDynamic toReturn = null;
            if (item!=null)
            {
                item.Skus = (await skuRepository.Query(p=>p.IdProduct==item.Id).Include(p => p.OptionValues).SelectAsync(false)).ToList();
                toReturn = new ProductDynamic(item, withDefaults);
            }

            return toReturn;
        }

        public async Task<ProductDynamic> UpdateProductAsync(ProductDynamic model)
        {
            ProductDynamic dbItem = null;

            return dbItem;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await productRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var skusCount = await skuRepository.Query(p => p.IdProduct == id && p.StatusCode!=RecordStatusCode.Deleted).SelectCountAsync();
                if(skusCount>0)
                {
                    throw new AppValidationException("The given product contains sub products. Delete sub products first.");
                }
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await productRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        #endregion
    }
}