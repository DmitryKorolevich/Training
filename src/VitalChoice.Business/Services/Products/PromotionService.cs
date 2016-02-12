using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.Products
{
    public class PromotionService : ExtendedEcommerceDynamicService<PromotionDynamic, Promotion, PromotionOptionType, PromotionOptionValue>, IPromotionService
    {
        private readonly IEcommerceRepositoryAsync<Promotion> _promotionRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly PromotionMapper _mapper;

        public PromotionService(
            IEcommerceRepositoryAsync<PromotionOptionValue> promotionOptionValueRepository,
            IEcommerceRepositoryAsync<Promotion> promotionRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, PromotionMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended loggerProvider, DirectMapper<Promotion> directMapper, DynamicExtensionsRewriter queryVisitor, ITransactionAccessor<EcommerceContext> transactionAccessor)
            : base(mapper, promotionRepository, promotionOptionValueRepository, bigStringRepositoryAsync, objectLogItemExternalService,
                loggerProvider, directMapper, queryVisitor, transactionAccessor)
        {
            _promotionRepository = promotionRepository;
            _skuRepository = skuRepository;
            _adminProfileRepository = adminProfileRepository;
            _mapper = mapper;
        }

        protected override Task<List<MessageInfo>> ValidateAsync(PromotionDynamic dynamic)
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

        protected override IQueryLite<Promotion> BuildQuery(IQueryLite<Promotion> query)
        {
            return query.Include(p => p.PromotionsToBuySkus)
                        .Include(p => p.PromotionsToGetSkus)
                        .Include(p => p.PromotionsToSelectedCategories);
        }

        protected override async Task AfterSelect(ICollection<Promotion> entities)
        {
            foreach (var entity in entities)
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
                            _skuRepository.Query(
                                p => skuIds.Contains(p.Id) && p.StatusCode != (int) RecordStatusCode.Deleted)
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
        }

        protected override bool LogObjectFullData => true;

        #region Promotions

        public async Task<PagedList<PromotionDynamic>> GetPromotionsAsync(PromotionFilter filter)
        {
            var conditions = new PromotionQuery().NotDeleted().WithValidFrom(filter.ValidFrom).WithValidTo(filter.ValidTo).
                WithDateStatus(filter.DateStatus).WithDescription(filter.SearchText).WithStatus(filter.Status);
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
            foreach (var item in result.Items)
            {
                item.OptionValues = new List<PromotionOptionValue>();
                item.OptionTypes = new List<PromotionOptionType>();
            }
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

        public Task<List<PromotionDynamic>> GetActivePromotions(CustomerType customerType)
        {
            return SelectAsync(
                new PromotionQuery().IsActive()
                    .WithExpiredType(ExpiredType.NotExpired)
                    .AllowCustomerType(customerType), withDefaults: true);
        }

        #endregion
    }
}