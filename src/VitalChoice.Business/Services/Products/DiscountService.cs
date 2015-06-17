using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Product;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Domain.Entities.eCommerce.Discounts;

namespace VitalChoice.Business.Services.Products
{
    public class DiscountService : IDiscountService
    {
        private readonly IEcommerceRepositoryAsync<DiscountOptionType> _discountOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<DiscountOptionValue> _discountOptionValueRepository;
        private readonly IEcommerceRepositoryAsync<Discount> _discountRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly EcommerceContext _context;
        private readonly ILogger _logger;

        public DiscountService(IEcommerceRepositoryAsync<DiscountOptionType> discountOptionTypeRepository,
            IEcommerceRepositoryAsync<DiscountOptionValue> discountOptionValueRepository,
            IEcommerceRepositoryAsync<Discount> discountRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            EcommerceContext context)
        {
            this._discountOptionTypeRepository = discountOptionTypeRepository;
            this._discountOptionValueRepository = discountOptionValueRepository;
            this._discountRepository = discountRepository;
            this._skuRepository = skuRepository;
            _context = context;
            _logger = LoggerService.GetDefault();
        }

        #region Discounts

        public async Task<PagedList<DiscountDynamic>> GetDiscountsAsync(DiscountFilter filter)
        {
            var conditions = new DiscountQuery().NotDeleted().WithText(filter.SearchText);
            var query = _discountRepository.Query(conditions);

            Func<IQueryable<Discount>, IOrderedQueryable<Discount>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;

            var result = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            PagedList<DiscountDynamic> toReturn = new PagedList<DiscountDynamic>(result.Items.Select(p => new DiscountDynamic(p)).ToList(), result.Count);

            return toReturn;
        }

        public async Task<DiscountDynamic> GetDiscountAsync(int id, bool withDefaults = false)
        {
            IQueryFluent<Discount> res = _discountRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .Include(p => p.DiscountTiers)
                .Include(p => p.DiscountsToSelectedSkus)
                .Include(p => p.DiscountsToSkus)
                .Include(p => p.DiscountsToCategories);
            var entity = (await res.SelectAsync(false)).FirstOrDefault();

            if (entity != null)
            {
                var skuIds = entity.DiscountsToSelectedSkus.Select(p => p.IdSku).ToList();
                var shortSkus = (await _skuRepository.Query(p => skuIds.Contains(p.Id) && p.StatusCode != RecordStatusCode.Deleted).Include(p => p.Product)
                    .SelectAsync(false)).Select(p => new ShortSkuInfo(p)).ToList();
                foreach (var sku in entity.DiscountsToSelectedSkus)
                {
                    foreach (var shortSku in shortSkus)
                    {
                        if (sku.Id == shortSku.Id)
                        {
                            sku.ShortSkuInfo = shortSku;
                            break;
                        }
                    }
                }
                foreach (var sku in entity.DiscountsToSkus)
                {
                    foreach (var shortSku in shortSkus)
                    {
                        if (sku.Id == shortSku.Id)
                        {
                            sku.ShortSkuInfo = shortSku;
                            break;
                        }
                    }
                }

                entity.OptionTypes = await _discountOptionTypeRepository.Query(o => o.IdDiscountType == entity.IdDiscountType).SelectAsync(false);
                Dictionary<int, DiscountOptionType> optionTypes = entity.OptionTypes.ToDictionary(o => o.Id, o => o);
                IncludeDiscountOptionTypes(entity, optionTypes);
                return new DiscountDynamic(entity, withDefaults);
            }

            return null;
        }

        public async Task<DiscountDynamic> UpdateDiscountAsync(DiscountDynamic model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            using (var uow = new EcommerceUnitOfWork())
            {
                Discount discount = null;
                int idDiscount = 0;
                try
                {
                    if (model.Id == 0)
                    {
                        idDiscount = (await InsertDiscount(model, uow)).Id;
                    }
                    discount = await UpdateDiscount(model, uow);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
                if (idDiscount != 0)
                    return await GetDiscountAsync(idDiscount);
                return new DiscountDynamic(discount);
            }
        }

        private async Task<Discount> InsertDiscount(DiscountDynamic model, EcommerceUnitOfWork uow)
        {
            var entity = model.ToEntity();
            if (entity != null)
            {
                var optionTypes = await _discountOptionTypeRepository.Query(o => o.IdDiscountType == model.DiscountType).SelectAsync(false);
                Dictionary<string, DiscountOptionType> optionTypesSorted = optionTypes.ToDictionary(o => o.Name, o => o);
                IncludeDiscountOptionTypesByName(entity, optionTypesSorted);

                entity.OptionTypes = new List<DiscountOptionType>();
                var discountRepository = uow.RepositoryAsync<Discount>();

                var result = await discountRepository.InsertGraphAsync(entity);
                await uow.SaveChangesAsync(CancellationToken.None);

                result.OptionTypes = optionTypes;
                return result;
            }
            return null;
        }

        private async Task<Discount> UpdateDiscount(DiscountDynamic model, EcommerceUnitOfWork uow)
        {
            var discountRepository = uow.RepositoryAsync<Discount>();
            var discountOptionValueRepository = uow.RepositoryAsync<DiscountOptionValue>();
            var discountTierRepository = uow.RepositoryAsync<DiscountTier>();
            var discountToSelectedSkuRepository = uow.RepositoryAsync<DiscountToSelectedSku>();

            var discountToSkuRepository = uow.RepositoryAsync<DiscountToSku>();
            var discountToCategoryRepository = uow.RepositoryAsync<DiscountToCategory>();

            var entity = (await discountRepository.Query(p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .SelectAsync()).FirstOrDefault();
            if (entity != null)
            {
                await discountOptionValueRepository.DeleteAllAsync(entity.OptionValues);

                entity.OptionTypes = await _discountOptionTypeRepository.Query(o => o.IdDiscountType == model.DiscountType).SelectAsync(false);

                model.UpdateEntity(entity);

                var selectedSkus = entity.DiscountsToSelectedSkus;
                entity.DiscountsToSelectedSkus = null;
                var skus = entity.DiscountsToSkus;
                entity.DiscountsToSkus = null;
                var categories = entity.DiscountsToCategories;
                entity.DiscountsToCategories = null;
                var discountTiers = entity.DiscountTiers;
                entity.DiscountTiers = null;

                var toReturn = await discountRepository.UpdateAsync(entity);

                var dbSelectedSkus = await discountToSelectedSkuRepository.Query(c => c.IdDiscount == entity.Id).SelectAsync();
                await discountToSelectedSkuRepository.DeleteAllAsync(dbSelectedSkus);
                await discountToSelectedSkuRepository.InsertRangeAsync(selectedSkus);

                var dbSkus = await discountToSkuRepository.Query(c => c.IdDiscount == entity.Id).SelectAsync();
                await discountToSkuRepository.DeleteAllAsync(dbSkus);
                await discountToSkuRepository.InsertRangeAsync(skus);

                var dbCategories = await discountToCategoryRepository.Query(c => c.IdDiscount == entity.Id).SelectAsync();
                await discountToCategoryRepository.DeleteAllAsync(dbCategories);
                await discountToCategoryRepository.InsertRangeAsync(categories);

                var dbDiscountTiers = await discountTierRepository.Query(c => c.IdDiscount == entity.Id).SelectAsync();
                await discountTierRepository.DeleteAllAsync(dbDiscountTiers);
                if (toReturn.IdDiscountType == DiscountType.Tiered && discountTiers != null && discountTiers.Count > 0)
                {
                    await discountTierRepository.InsertRangeAsync(discountTiers);
                }

                await uow.SaveChangesAsync(CancellationToken.None);

                toReturn.DiscountsToSelectedSkus = selectedSkus;
                toReturn.DiscountsToSkus = skus;
                toReturn.DiscountsToCategories = categories;
                toReturn.DiscountTiers = discountTiers;
                return toReturn;
            }
            return null;
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _discountRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _discountRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        #endregion

        private static void IncludeDiscountOptionTypes(Discount item, Dictionary<int, DiscountOptionType> optionTypes)
        {
            foreach (var value in item.OptionValues)
            {
                DiscountOptionType optionType;
                value.OptionType = optionTypes.TryGetValue(value.IdOptionType, out optionType) ? optionType : null;
            }
        }

        private static void IncludeDiscountOptionTypesByName(Discount item,
            Dictionary<string, DiscountOptionType> optionTypes)
        {
            var forRemove = new List<DiscountOptionValue>();
            foreach (var value in item.OptionValues)
            {
                DiscountOptionType optionType;
                optionTypes.TryGetValue(value.OptionType.Name, out optionType);
                if (optionType == null)
                {
                    forRemove.Add(value);
                }
                else
                {
                    value.OptionType = null;
                    value.IdOptionType = optionType.Id;
                }
            }
            foreach (var forRemoveItem in forRemove)
            {
                item.OptionValues.Remove(forRemoveItem);
            }
        }
    }
}