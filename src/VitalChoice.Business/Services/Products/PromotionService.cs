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
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
    public class PromotionService : EcommerceDynamicObjectService<PromotionDynamic, Promotion, PromotionOptionType, PromotionOptionValue>, IPromotionService
    {
        private readonly IEcommerceRepositoryAsync<Promotion> _promotionRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly PromotionMapper _mapper;

        public PromotionService(IEcommerceRepositoryAsync<PromotionOptionType> promotionOptionTypeRepository,
            IEcommerceRepositoryAsync<PromotionOptionValue> promotionOptionValueRepository,
            IEcommerceRepositoryAsync<Promotion> promotionRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, PromotionMapper mapper)
            : base(mapper, promotionRepository, promotionOptionTypeRepository, promotionOptionValueRepository, bigStringRepositoryAsync)
        {
            _promotionRepository = promotionRepository;
            _skuRepository = skuRepository;
            _adminProfileRepository = adminProfileRepository;
            _mapper = mapper;
        }

        protected override Task<List<MessageInfo>> Validate(PromotionDynamic dynamic)
        {
            List<MessageInfo> errors = new List<MessageInfo>();
            if (dynamic.IdObjectType == (int)PromotionType.BuyXGetY)
            {
                if (dynamic.PromotionsToBuySkus == null || dynamic.PromotionsToBuySkus.Count == 0)
                {
                    errors.AddRange(CreateError().Error("At least one skus should be specified for buy area.").Build());
                }
                if (dynamic.PromotionsToGetSkus == null || dynamic.PromotionsToGetSkus.Count == 0)
                {
                    errors.AddRange(CreateError().Error("At least one skus should be specified for get area.").Build());
                }
            }
            return Task.FromResult(errors);
        }

        protected override IQueryFluent<Promotion> BuildQuery(IQueryFluent<Promotion> query)
        {
            return query.Include(p => p.PromotionsToBuySkus)
                        .Include(p => p.PromotionsToGetSkus)
                        .Include(p => p.PromotionsToSelectedCategories);
        }

        protected override async Task AfterSelect(Promotion entity)
        {
            var skuIds = new HashSet<int>(entity.PromotionsToBuySkus.Select(p => p.IdSku));
            foreach (var id in entity.PromotionsToGetSkus.Select(p => p.IdSku))
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
                foreach (var sku in entity.PromotionsToBuySkus)
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
                foreach (var sku in entity.PromotionsToGetSkus)
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
        }

        protected override async Task BeforeEntityChangesAsync(PromotionDynamic model, Promotion entity, IUnitOfWorkAsync uow)
        {
            var promotionToSelectedSkuRepository = uow.RepositoryAsync<PromotionToBuySku>();
            var promotionToSkuRepository = uow.RepositoryAsync<PromotionToGetSku>();
            var promotionToSelectedCategoryRepository = uow.RepositoryAsync<PromotionToSelectedCategory>();

            await promotionToSelectedSkuRepository.DeleteAllAsync(entity.PromotionsToBuySkus);
            await promotionToSkuRepository.DeleteAllAsync(entity.PromotionsToGetSkus);
            await promotionToSelectedCategoryRepository.DeleteAllAsync(entity.PromotionsToSelectedCategories);
        }

        protected override async Task AfterEntityChangesAsync(PromotionDynamic model, Promotion entity, IUnitOfWorkAsync uow)
        {
            var promotionToSelectedSkuRepository = uow.RepositoryAsync<PromotionToBuySku>();
            var promotionToSkuRepository = uow.RepositoryAsync<PromotionToGetSku>();
            var promotionToSelectedCategoryRepository = uow.RepositoryAsync<PromotionToSelectedCategory>();

            await promotionToSelectedSkuRepository.InsertRangeAsync(entity.PromotionsToBuySkus);
            await promotionToSkuRepository.InsertRangeAsync(entity.PromotionsToGetSkus);
            await promotionToSelectedCategoryRepository.InsertRangeAsync(entity.PromotionsToSelectedCategories);
        }

        #region Promotions

        public async Task<PagedList<PromotionDynamic>> GetPromotionsAsync(PromotionFilter filter)
        {
            var conditions = new PromotionQuery().NotDeleted().WithDescription(filter.SearchText).WithStatus(filter.Status);
            var query = _promotionRepository.Query(conditions);

            Func<IQueryable<Promotion>, IOrderedQueryable<Promotion>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case PromotionSortPath.Description:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Description)
                                : x.OrderByDescending(y => y.Description);
                    break;
                case PromotionSortPath.IdObjectType:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdObjectType)
                                : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case PromotionSortPath.Assigned:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Assigned)
                                : x.OrderByDescending(y => y.Assigned);
                    break;
                case PromotionSortPath.StartDate:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StartDate)
                                : x.OrderByDescending(y => y.StartDate);
                    break;
                case PromotionSortPath.ExpirationDate:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ExpirationDate)
                                : x.OrderByDescending(y => y.ExpirationDate);
                    break;
                case PromotionSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
            }

            var result = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            PagedList<PromotionDynamic> toReturn = new PagedList<PromotionDynamic>(result.Items.Select(p => _mapper.FromEntity(p)).ToList(), result.Count);
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