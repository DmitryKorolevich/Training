using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
    public class DiscountService : EcommerceDynamicObjectService<DiscountDynamic, Discount, DiscountOptionType, DiscountOptionValue>, IDiscountService
    {
        private readonly IEcommerceRepositoryAsync<Discount> _discountRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly DiscountMapper _mapper;

        public DiscountService(IEcommerceRepositoryAsync<DiscountOptionType> discountOptionTypeRepository,
            IEcommerceRepositoryAsync<DiscountOptionValue> discountOptionValueRepository,
            IEcommerceRepositoryAsync<Discount> discountRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, DiscountMapper mapper)
            : base(mapper, discountRepository, discountOptionTypeRepository, discountOptionValueRepository, bigStringRepositoryAsync)
        {
            _discountRepository = discountRepository;
            _skuRepository = skuRepository;
            _adminProfileRepository = adminProfileRepository;
            _mapper = mapper;
        }

        protected override async Task<List<MessageInfo>> Validate(DiscountDynamic dynamic)
        {
            var codeDublicatesExist =
                await
                    _discountRepository.Query(
                        p => p.Code == dynamic.Code && p.Id != dynamic.Id && p.StatusCode != RecordStatusCode.Deleted)
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

        protected override IQueryFluent<Discount> BuildQuery(IQueryFluent<Discount> query)
        {
            return query.Include(p => p.DiscountTiers)
                .Include(p => p.DiscountsToSelectedSkus)
                .Include(p => p.DiscountsToSkus)
                .Include(p => p.DiscountsToCategories);
        }

        protected override async Task AfterSelect(Discount entity)
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
                        _skuRepository.Query(p => skuIds.Contains(p.Id) && p.StatusCode != RecordStatusCode.Deleted)
                            .Include(p => p.Product)
                            .SelectAsync(false)).Select(p => new ShortSkuInfo(p)).ToList();
                foreach (var sku in entity.DiscountsToSelectedSkus)
                {
                    foreach (var shortSku in shortSkus)
                    {
                        if (sku.IdSku == shortSku.Id)
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
                        if (sku.IdSku == shortSku.Id)
                        {
                            sku.ShortSkuInfo = shortSku;
                            break;
                        }
                    }
                }
            }
            entity.DiscountTiers = entity.DiscountTiers.OrderBy(p => p.Order).ToList();
        }

        protected override async Task BeforeEntityChangesAsync(DiscountDynamic model, Discount entity, IUnitOfWorkAsync uow)
        {
            var discountTierRepository = uow.RepositoryAsync<DiscountTier>();
            var discountToSelectedSkuRepository = uow.RepositoryAsync<DiscountToSelectedSku>();
            var discountToSkuRepository = uow.RepositoryAsync<DiscountToSku>();
            var discountToCategoryRepository = uow.RepositoryAsync<DiscountToCategory>();

            await discountToSelectedSkuRepository.DeleteAllAsync(entity.DiscountsToSelectedSkus);
            await discountToSkuRepository.DeleteAllAsync(entity.DiscountsToSkus);
            await discountToCategoryRepository.DeleteAllAsync(entity.DiscountsToCategories);
            await discountTierRepository.DeleteAllAsync(entity.DiscountTiers);
        }

        protected override async Task AfterEntityChangesAsync(DiscountDynamic model, Discount entity, IUnitOfWorkAsync uow)
        {
            var discountTierRepository = uow.RepositoryAsync<DiscountTier>();
            var discountToSelectedSkuRepository = uow.RepositoryAsync<DiscountToSelectedSku>();
            var discountToSkuRepository = uow.RepositoryAsync<DiscountToSku>();
            var discountToCategoryRepository = uow.RepositoryAsync<DiscountToCategory>();

            await discountToSelectedSkuRepository.InsertRangeAsync(entity.DiscountsToSelectedSkus);
            await discountToSkuRepository.InsertRangeAsync(entity.DiscountsToSkus);
            await discountToCategoryRepository.InsertRangeAsync(entity.DiscountsToCategories);
            if (entity.IdObjectType == (int)DiscountType.Tiered && entity.DiscountTiers != null && entity.DiscountTiers.Count > 0)
            {
                await discountTierRepository.InsertRangeAsync(entity.DiscountTiers);
            }
        }

        #region Discounts

        public async Task<PagedList<DiscountDynamic>> GetDiscountsAsync(DiscountFilter filter)
        {
            var conditions = new DiscountQuery().NotDeleted().WithText(filter.SearchText).WithStatus(filter.Status);
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
                case DiscountSortPath.IdDiscountType:
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

        #endregion
    }
}