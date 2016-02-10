using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Queries.Products;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Disocunts;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.Products
{
    public class DiscountService : ExtendedEcommerceDynamicService<DiscountDynamic, Discount, DiscountOptionType, DiscountOptionValue>, IDiscountService
    {
        private readonly IEcommerceRepositoryAsync<Discount> _discountRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly DiscountMapper _mapper;

        public DiscountService(
            IEcommerceRepositoryAsync<DiscountOptionValue> discountOptionValueRepository,
            IEcommerceRepositoryAsync<Discount> discountRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, DiscountMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended loggerProvider, DirectMapper<Discount> directMapper, DynamicExtensionsRewriter queryVisitor, EcommerceContext dbContext)
            : base(mapper, discountRepository, discountOptionValueRepository, bigStringRepositoryAsync, objectLogItemExternalService, 
                  loggerProvider, directMapper, queryVisitor, dbContext)
        {
            _discountRepository = discountRepository;
            _skuRepository = skuRepository;
            _adminProfileRepository = adminProfileRepository;
            _mapper = mapper;
        }

        protected override async Task<List<MessageInfo>> ValidateAsync(DiscountDynamic dynamic)
        {
            var codeDublicatesExist =
                await
                    _discountRepository.Query(
                        p => p.Code == dynamic.Code && p.Id != dynamic.Id && p.StatusCode != (int)RecordStatusCode.Deleted)
                        .SelectAnyAsync();
            if (codeDublicatesExist)
            {
                return dynamic.CreateError()
                    .Property(d => d.Code)
                    .Error("Discount with the same Code already exists, please use a unique Code.")
                    .Build();
            }
            return new List<MessageInfo>();
        }

        protected override IQueryLite<Discount> BuildQuery(IQueryLite<Discount> query)
        {
            return query.Include(p => p.DiscountTiers)
                .Include(p => p.DiscountsToSelectedSkus)
                .Include(p => p.DiscountsToSkus)
                .Include(p => p.DiscountsToCategories)
                .Include(p => p.DiscountsToSelectedCategories);
        }

        protected override async Task AfterSelect(ICollection<Discount> entities)
        {
            foreach (var entity in entities)
            {
                var skuIds = new HashSet<int>(entity.DiscountsToSelectedSkus.Select(p => p.IdSku));
                foreach (var id in entity.DiscountsToSkus.Select(p => p.IdSku))
                {
                    skuIds.Add(id);
                }
                if (skuIds.Count > 0)
                {
                    var shortSkus =
                        (await
                            _skuRepository.Query(
                                p => skuIds.Contains(p.Id) && p.StatusCode != (int) RecordStatusCode.Deleted)
                                .Include(p => p.Product)
                                .SelectAsync(false)).Select(p => new ShortSkuInfo(p)).ToDictionary(s => s.Id);
                    foreach (var sku in entity.DiscountsToSelectedSkus)
                    {
                        ShortSkuInfo skuInfo;
                        if (shortSkus.TryGetValue(sku.IdSku, out skuInfo))
                            sku.ShortSkuInfo = skuInfo;
                    }
                    foreach (var sku in entity.DiscountsToSkus)
                    {
                        ShortSkuInfo skuInfo;
                        if (shortSkus.TryGetValue(sku.IdSku, out skuInfo))
                            sku.ShortSkuInfo = skuInfo;
                    }
                }
                entity.DiscountTiers = entity.DiscountTiers.OrderBy(p => p.Order).ToArray();
            }
        }

        protected override async Task BeforeEntityChangesAsync(DiscountDynamic model, Discount entity, IUnitOfWorkAsync uow)
        {
            var discountTierRepository = uow.RepositoryAsync<DiscountTier>();
            //var discountToSelectedSkuRepository = uow.RepositoryAsync<DiscountToSelectedSku>();
            //var discountToSkuRepository = uow.RepositoryAsync<DiscountToSku>();
            //var discountToCategoryRepository = uow.RepositoryAsync<DiscountToCategory>();
            //var discountToSelectedCategoryRepository = uow.RepositoryAsync<DiscountToSelectedCategory>();

            //await discountToSelectedSkuRepository.DeleteAllAsync(entity.DiscountsToSelectedSkus);
            //await discountToSkuRepository.DeleteAllAsync(entity.DiscountsToSkus);
            //await discountToCategoryRepository.DeleteAllAsync(entity.DiscountsToCategories);
            //await discountToSelectedCategoryRepository.DeleteAllAsync(entity.DiscountsToSelectedCategories);
            await discountTierRepository.DeleteAllAsync(entity.DiscountTiers);
        }

        protected override async Task AfterEntityChangesAsync(DiscountDynamic model, Discount updated, Discount initial,
            IUnitOfWorkAsync uow)
        {
            var discountTierRepository = uow.RepositoryAsync<DiscountTier>();
            //var discountToSelectedSkuRepository = uow.RepositoryAsync<DiscountToSelectedSku>();
            //var discountToSkuRepository = uow.RepositoryAsync<DiscountToSku>();
            //var discountToCategoryRepository = uow.RepositoryAsync<DiscountToCategory>();
            //var discountToSelectedCategoryRepository = uow.RepositoryAsync<DiscountToSelectedCategory>();

            //await discountToSelectedSkuRepository.DeleteAllAsync(
            //    initial.DiscountsToSelectedSkus.ExceptKeyedWith(updated.DiscountsToSelectedSkus,
            //        item => item.IdSku));

            //await discountToSkuRepository.DeleteAllAsync(initial.DiscountsToSkus.ExceptKeyedWith(updated.DiscountsToSkus,
            //    item => item.IdSku));

            //await discountToCategoryRepository.DeleteAllAsync(initial.DiscountsToCategories.ExceptKeyedWith(updated.DiscountsToCategories,
            //    item => item.IdCategory));

            //await
            //    discountToSelectedCategoryRepository.DeleteAllAsync(
            //        initial.DiscountsToSelectedCategories.ExceptKeyedWith(updated.DiscountsToSelectedCategories,
            //            item => item.IdCategory));

            if (updated.IdObjectType == (int) DiscountType.Tiered && updated.DiscountTiers != null && updated.DiscountTiers.Count > 0)
            {
                await discountTierRepository.InsertRangeAsync(updated.DiscountTiers);
            }
        }

        protected override bool LogObjectFullData { get { return true; } }

        #region Discounts

        public async Task<PagedList<DiscountDynamic>> GetDiscountsAsync(DiscountFilter filter)
        {
            var conditions = new DiscountQuery().NotDeleted().WithValidFrom(filter.ValidFrom).WithValidTo(filter.ValidTo).
                WithDateStatus(filter.DateStatus).WithText(filter.SearchText).WithCode(filter.Code).WithStatus(filter.Status);
            var query = _discountRepository.Query(conditions);

            Func<IQueryable<Discount>, IOrderedQueryable<Discount>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case DiscountSortPath.Code:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Code)
                                : x.OrderByDescending(y => y.Code);
                    break;
                case DiscountSortPath.Description:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Description)
                                : x.OrderByDescending(y => y.Description);
                    break;
                case DiscountSortPath.IdObjectType:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdObjectType)
                                : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case DiscountSortPath.Assigned:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Assigned)
                                : x.OrderByDescending(y => y.Assigned);
                    break;
                case DiscountSortPath.StartDate:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StartDate)
                                : x.OrderByDescending(y => y.StartDate);
                    break;
                case DiscountSortPath.ExpirationDate:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ExpirationDate)
                                : x.OrderByDescending(y => y.ExpirationDate);
                    break;
                case DiscountSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
            }

            var result = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            foreach(var item in result.Items)
            {
                item.OptionValues = new List<DiscountOptionValue>();
                item.OptionTypes = new List<DiscountOptionType>();
            }
            PagedList<DiscountDynamic> toReturn = new PagedList<DiscountDynamic>(result.Items.Select(p => _mapper.FromEntity(p)).ToList(), result.Count);
            if (toReturn.Items.Any())
            {
                var ids = result.Items.Select(p => p.IdAddedBy).ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdAddedBy == profile.Id)
                        {
                            item.Data.AddedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }

        public Task<DiscountDynamic> GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Task.FromResult<DiscountDynamic>(null);
            return SelectFirstAsync(new DiscountQuery().WithEqualCode(code).NotDeleted(), withDefaults: true);
        }

        #endregion
    }
}