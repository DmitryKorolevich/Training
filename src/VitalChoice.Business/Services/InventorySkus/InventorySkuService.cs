using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.InventorySkus;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Interfaces.Services;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.InventorySkus
{
    public class InventorySkuService : ExtendedEcommerceDynamicService<InventorySkuDynamic, InventorySku, InventorySkuOptionType, InventorySkuOptionValue>,
        IInventorySkuService
    {
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<SkuToInventorySku> _skuToInventorySkuRepository;
        private readonly IInventorySkuCategoryService _inventorySkuCategoryService;
        private readonly SPEcommerceRepository _sPEcommerceRepository;
        private readonly ISettingService _settingService;

        public InventorySkuService(InventorySkuMapper mapper,
            IEcommerceRepositoryAsync<InventorySku> inventorySkuRepository,
            IEcommerceRepositoryAsync<InventorySkuOptionValue> inventorySkuValueRepositoryAsync,
            IInventorySkuCategoryService inventorySkuCategoryService,
            SPEcommerceRepository sPEcommerceRepository,
            ISettingService settingService,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            DirectMapper<InventorySku> directMapper,
            DynamicExtensionsRewriter queryVisitor,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<SkuToInventorySku> skuToInventorySkuRepository,
            ILoggerProviderExtended loggerProvider) : base(
                mapper, inventorySkuRepository, inventorySkuValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor, transactionAccessor)
        {
            _adminProfileRepository = adminProfileRepository;
            _skuToInventorySkuRepository = skuToInventorySkuRepository;
            _inventorySkuCategoryService = inventorySkuCategoryService;
            _sPEcommerceRepository = sPEcommerceRepository;
            _settingService = settingService;
        }

        public async Task<PagedList<InventorySkuListItemModel>> GetInventorySkusAsync(InventorySkuFilter filter)
        {
            var conditions =new InventorySkuQuery().NotDeleted().WithIds(filter.Ids).WithStatus(filter.StatusCode).WithCode(filter.Code).WithDescription(filter.Description);
            var query = ObjectRepository.Query(conditions);

            Func<IQueryable<InventorySku>, IOrderedQueryable<InventorySku>> sortable = x => x.OrderBy(y => y.Code);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case InventorySkuSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case InventorySkuSortPath.Code:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Code)
                                : x.OrderByDescending(y => y.Code);
                    break;
                case InventorySkuSortPath.Description:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Description)
                                : x.OrderByDescending(y => y.Description);
                    break;
                case InventorySkuSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
                case InventorySkuSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
            }

            PagedList<InventorySku> result = null;
            if (filter.Paging != null)
            {
                result = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = await query.OrderBy(sortable).SelectAsync(false);
                result =new PagedList<InventorySku>()
                {
                    Items = items,
                    Count = items.Count,
                };
            }

            var toReturn=new PagedList<InventorySkuListItemModel>()
            {
                Items = result.Items.Select(p=>new InventorySkuListItemModel(p)).ToList(),
                Count = result.Count,
            };

            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value).Distinct().ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdEditedBy == profile.Id)
                        {
                            item.EditedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }

        public async Task<Dictionary<int,List<SkuToInventorySku>>> GetAssignedInventorySkusAsync(ICollection<int> skuIds)
        {
            skuIds = skuIds.Distinct().ToList();
            var items = await _skuToInventorySkuRepository.Query(p => skuIds.Contains(p.IdSku)).SelectAsync(false);
            var toReturn =new Dictionary<int, List<SkuToInventorySku>>();
            foreach (var skuToInventorySku in items)
            {
                List<SkuToInventorySku> assignedInventories;
                toReturn.TryGetValue(skuToInventorySku.IdSku, out assignedInventories);
                if (assignedInventories == null)
                {
                    assignedInventories = new List<SkuToInventorySku>();
                    toReturn.Add(skuToInventorySku.IdSku, assignedInventories);
                }
                assignedInventories.Add(skuToInventorySku);
            }
            return toReturn;
        }

        public async Task<ICollection<InventorySkuUsageReportItem>> GetInventorySkuUsageReportAsync(InventorySkuUsageReportFilter filter)
        {
            var toReturn = new List<InventorySkuUsageReportItem>();
            var data = await _sPEcommerceRepository.GetInventorySkusUsageReportAsync(filter);

            foreach (var inventorySkuUsageRawReportItem in data)
            {
                var item = toReturn.FirstOrDefault(p => p.IdSku == inventorySkuUsageRawReportItem.IdSku);
                if (item == null)
                {
                    item=new InventorySkuUsageReportItem();
                    item.IdSku = inventorySkuUsageRawReportItem.IdSku;
                    item.SkuCode = inventorySkuUsageRawReportItem.SkuCode;
                    item.TotalSkuQuantity = inventorySkuUsageRawReportItem.TotalSkuQuantity;
                    item.BornDate = inventorySkuUsageRawReportItem.BornDate;
                    item.InventorySkuChannel = inventorySkuUsageRawReportItem.InventorySkuChannel;
                    toReturn.Add(item);
                }

                if (inventorySkuUsageRawReportItem.IdInventorySku.HasValue)
                {
                    var inventorySku = new SubInventorySkuUsageReportItem();
                    inventorySku.IdInventorySku = inventorySkuUsageRawReportItem.IdInventorySku.Value;
                    inventorySku.TotalInvSkuQuantity = inventorySkuUsageRawReportItem.TotalInvSkuQuantity ?? 0;
                    inventorySku.InvSkuCode = inventorySkuUsageRawReportItem.InvSkuCode;
                    inventorySku.InvDescription = inventorySkuUsageRawReportItem.InvDescription;
                    inventorySku.IdInventorySkuCategory = inventorySkuUsageRawReportItem.IdInventorySkuCategory;

                    inventorySku.ProductSource = inventorySkuUsageRawReportItem.ProductSource;
                    inventorySku.TotalInvQuantityWithInvCorrection = (inventorySkuUsageRawReportItem.InvQuantity ?? 0) * inventorySku.TotalInvSkuQuantity;
                    inventorySku.UnitOfMeasure = inventorySkuUsageRawReportItem.UnitOfMeasure;
                    inventorySku.TotalUnitOfMeasureAmount = (inventorySkuUsageRawReportItem.UnitOfMeasureAmount ?? 0) * inventorySku.TotalInvSkuQuantity;
                    inventorySku.PurchaseUnitOfMeasure = inventorySkuUsageRawReportItem.PurchaseUnitOfMeasure;
                    inventorySku.PurchaseUnitOfMeasureAmount = inventorySkuUsageRawReportItem.PurchaseUnitOfMeasureAmount ?? 0;

                    item.InventorySkus.Add(inventorySku);
                }
            }

            toReturn = toReturn.OrderBy(p => p.SkuCode).ToList();

            return toReturn;
        }

        public async Task<ICollection<InventorySkuUsageReportItemForExport>> GetInventorySkuUsageReportForExportAsync(InventorySkuUsageReportFilter filter)
        {
            var toReturn = new List<InventorySkuUsageReportItemForExport>();
            var data = await GetInventorySkuUsageReportAsync(filter);
            
            var categoryTree = new InventorySkuCategory();
            categoryTree.SubCategories = await _inventorySkuCategoryService.GetCategoriesTreeAsync(new InventorySkuCategoryTreeFilter());

            var lookups = await _settingService.GetLookupsAsync(SettingConstants.INVENTORY_SKU_LOOKUP_NAMES.Split(','));

            foreach (var inventorySkuUsageReportItem in data)
            {
                InventorySkuUsageReportItemForExport item=new InventorySkuUsageReportItemForExport();
                item.IdSku = inventorySkuUsageReportItem.IdSku;
                item.SkuCode = inventorySkuUsageReportItem.SkuCode;
                item.TotalSkuQuantity = inventorySkuUsageReportItem.TotalSkuQuantity;
                item.BornDate = inventorySkuUsageReportItem.BornDate;
                item.Assemble = inventorySkuUsageReportItem.Assemble.HasValue ? (inventorySkuUsageReportItem.Assemble.Value ? "yes" : "no") : null;

                var lookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_CHANNEL_NAME);
                if (lookup != null)
                {
                    item.InventorySkuChannel=lookup.LookupVariants.FirstOrDefault(p=>p.Id==inventorySkuUsageReportItem.InventorySkuChannel)?.ValueVariant;
                }

                toReturn.Add(item);

                foreach (var subInventorySkuUsageReportItem in inventorySkuUsageReportItem.InventorySkus)
                {
                    item = new InventorySkuUsageReportItemForExport();
                    item.IdInventorySku = subInventorySkuUsageReportItem.IdInventorySku;
                    item.TotalInvSkuQuantity = subInventorySkuUsageReportItem.TotalInvSkuQuantity;
                    item.InvSkuCode = subInventorySkuUsageReportItem.InvSkuCode;
                    item.InvDescription = subInventorySkuUsageReportItem.InvDescription;
                    item.InventorySkuCategory = FindUICategory(categoryTree, subInventorySkuUsageReportItem.IdInventorySkuCategory)?.Name;

                    lookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_PRODUCT_SOURCE_NAME);
                    if (lookup != null)
                    {
                        item.ProductSource = lookup.LookupVariants.FirstOrDefault(p => p.Id == subInventorySkuUsageReportItem.ProductSource)?.ValueVariant;
                    }
                    lookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_UNIT_OF_MEASURE_NAME);
                    if (lookup != null)
                    {
                        item.UnitOfMeasure = lookup.LookupVariants.FirstOrDefault(p => p.Id == subInventorySkuUsageReportItem.UnitOfMeasure)?.ValueVariant;
                    }
                    lookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_PURCHASE_UNIT_OF_MEASURE_NAME);
                    if (lookup != null)
                    {
                        item.PurchaseUnitOfMeasure = lookup.LookupVariants.FirstOrDefault(p => p.Id == subInventorySkuUsageReportItem.PurchaseUnitOfMeasure)?.ValueVariant;
                    }

                    item.TotalInvQuantityWithInvCorrection=subInventorySkuUsageReportItem.TotalInvQuantityWithInvCorrection;
                    item.TotalUnitOfMeasureAmount = subInventorySkuUsageReportItem.TotalUnitOfMeasureAmount;
                    item.PurchaseUnitOfMeasureAmount = subInventorySkuUsageReportItem.PurchaseUnitOfMeasureAmount;

                    toReturn.Add(item);
                }
            }

            return toReturn;
        }

        private InventorySkuCategory FindUICategory(InventorySkuCategory root, int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            if (root.Id == id)
            {
                if (root.ParentId.HasValue)
                {
                    return root;
                }
            }
            else
            {
                return root.SubCategories.Select(subCategory => FindUICategory(subCategory, id)).FirstOrDefault(res => res != null);
            }

            return null;
        }
    }
}