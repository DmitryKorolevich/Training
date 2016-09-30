﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation.Resources;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Queries.InventorySkus;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
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
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.ObjectMapping.Base;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Interfaces.Services.Products;
using System.Text.RegularExpressions;

namespace VitalChoice.Business.Services.InventorySkus
{
    public class InventorySkuService : ExtendedEcommerceDynamicService<InventorySkuDynamic, InventorySku, InventorySkuOptionType, InventorySkuOptionValue>,
        IInventorySkuService
    {
        private static readonly Regex QTYRegex = new Regex(@"\([0-9]+\)", RegexOptions.Compiled | RegexOptions.RightToLeft);

        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<SkuToInventorySku> _skuToInventorySkuRepository;
        private readonly IInventorySkuCategoryService _inventorySkuCategoryService;
        private readonly SpEcommerceRepository _sPEcommerceRepository;
        private readonly ISettingService _settingService;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IProductService _productService;
        private readonly ReferenceData _referenceData;

        public InventorySkuService(InventorySkuMapper mapper,
            IEcommerceRepositoryAsync<InventorySku> inventorySkuRepository,
            IEcommerceRepositoryAsync<InventorySkuOptionValue> inventorySkuValueRepositoryAsync,
            IInventorySkuCategoryService inventorySkuCategoryService,
            SpEcommerceRepository sPEcommerceRepository,
            ISettingService settingService,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IProductService productService,
            ReferenceData referenceData,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            DynamicExtensionsRewriter queryVisitor,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<SkuToInventorySku> skuToInventorySkuRepository,
            ILoggerFactory loggerProvider, IDynamicEntityOrderingExtension<InventorySku> orderingExtension) : base(
                mapper, inventorySkuRepository, inventorySkuValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor, transactionAccessor, orderingExtension)
        {
            _adminProfileRepository = adminProfileRepository;
            _skuToInventorySkuRepository = skuToInventorySkuRepository;
            _inventorySkuCategoryService = inventorySkuCategoryService;
            _sPEcommerceRepository = sPEcommerceRepository;
            _settingService = settingService;
            _skuRepository = skuRepository;
            _productService = productService;
            _referenceData = referenceData;
        }

        public async Task<PagedList<InventorySkuListItemModel>> GetInventorySkusAsync(InventorySkuFilter filter)
        {
            var conditions = new InventorySkuQuery().NotDeleted().WithIds(filter.Ids).WithStatus(filter.StatusCode)
                .WithExactCode(filter.ExactCode)
                .WithCode(filter.Code)
                .WithDescription(filter.Description);
            var query = ObjectRepository.Query(conditions);

            Func<IQueryable<InventorySku>, IOrderedQueryable<InventorySku>> sortable = x => x.OrderBy(y => y.Code);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case InventorySkuSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case InventorySkuSortPath.Code:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Code)
                                : x.OrderByDescending(y => y.Code);
                    break;
                case InventorySkuSortPath.Description:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Description)
                                : x.OrderByDescending(y => y.Description);
                    break;
                case InventorySkuSortPath.StatusCode:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
                case InventorySkuSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
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
                result = new PagedList<InventorySku>()
                {
                    Items = items,
                    Count = items.Count,
                };
            }

            var toReturn = new PagedList<InventorySkuListItemModel>()
            {
                Items = result.Items.Select(p => new InventorySkuListItemModel(p)).ToList(),
                Count = result.Count,
            };

            if (toReturn.Items.Count > 0)
            {
                var ids = toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value).Distinct().ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
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

        public async Task<Dictionary<int, List<SkuToInventorySku>>> GetAssignedInventorySkusAsync(IEnumerable<int> skuIds)
        {
            skuIds = skuIds.Distinct().ToList();
            var items = await _skuToInventorySkuRepository.Query(p => skuIds.Contains(p.IdSku)).SelectAsync(false);
            //var toReturn = new Dictionary<int, List<SkuToInventorySku>>();
            //foreach (var skuToInventorySku in items)
            //{
            //    List<SkuToInventorySku> assignedInventories;
            //    toReturn.TryGetValue(skuToInventorySku.IdSku, out assignedInventories);
            //    if (assignedInventories == null)
            //    {
            //        assignedInventories = new List<SkuToInventorySku>();
            //        toReturn.Add(skuToInventorySku.IdSku, assignedInventories);
            //    }
            //    assignedInventories.Add(skuToInventorySku);
            //}
            //return toReturn;
            return items.GroupBy(inv => inv.IdSku).ToDictionary(inv => inv.Key, g => g.ToList());
        }

        #region Reports

        public async Task<ICollection<InventorySkuUsageReportItem>> GetInventorySkuUsageReportAsync(InventorySkuUsageReportFilter filter)
        {
            var toReturn = new List<InventorySkuUsageReportItem>();
            var data = await _sPEcommerceRepository.GetInventorySkusUsageReportAsync(filter);

            var categoryTree = new InventorySkuCategory();
            categoryTree.SubCategories = await _inventorySkuCategoryService.GetCategoriesTreeAsync(new InventorySkuCategoryTreeFilter());

            foreach (var inventorySkuUsageRawReportItem in data)
            {
                var item = toReturn.FirstOrDefault(p => p.IdSku == inventorySkuUsageRawReportItem.IdSku);
                if (item == null)
                {
                    item = new InventorySkuUsageReportItem();
                    item.IdSku = inventorySkuUsageRawReportItem.IdSku;
                    item.SkuCode = inventorySkuUsageRawReportItem.SkuCode;
                    item.TotalSkuQuantity = inventorySkuUsageRawReportItem.TotalSkuQuantity;
                    item.BornDate = inventorySkuUsageRawReportItem.BornDate;
                    item.Assemble = inventorySkuUsageRawReportItem.Assemble;
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
                    inventorySku.InventorySkuCategory = FindUICategory(categoryTree, inventorySkuUsageRawReportItem.IdInventorySkuCategory)?.Name;

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

            var lookups = await _settingService.GetLookupsAsync(SettingConstants.INVENTORY_SKU_LOOKUP_NAMES.Split(','));

            foreach (var inventorySkuUsageReportItem in data)
            {
                InventorySkuUsageReportItemForExport item = new InventorySkuUsageReportItemForExport();
                item.IdSku = inventorySkuUsageReportItem.IdSku;
                item.SkuCode = inventorySkuUsageReportItem.SkuCode;
                item.TotalSkuQuantity = inventorySkuUsageReportItem.TotalSkuQuantity;
                item.BornDate = inventorySkuUsageReportItem.BornDate;
                item.Assemble = inventorySkuUsageReportItem.Assemble.HasValue ? (inventorySkuUsageReportItem.Assemble.Value ? "yes" : "no") : null;

                var lookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_CHANNEL_NAME);
                if (lookup != null)
                {
                    item.InventorySkuChannel = lookup.LookupVariants.FirstOrDefault(p => p.Id == inventorySkuUsageReportItem.InventorySkuChannel)?.ValueVariant;
                }

                toReturn.Add(item);

                foreach (var subInventorySkuUsageReportItem in inventorySkuUsageReportItem.InventorySkus)
                {
                    item = new InventorySkuUsageReportItemForExport();
                    item.IdInventorySku = subInventorySkuUsageReportItem.IdInventorySku;
                    item.TotalInvSkuQuantity = subInventorySkuUsageReportItem.TotalInvSkuQuantity;
                    item.InvSkuCode = subInventorySkuUsageReportItem.InvSkuCode;
                    item.InvDescription = subInventorySkuUsageReportItem.InvDescription;
                    item.InventorySkuCategory = subInventorySkuUsageReportItem.InventorySkuCategory;

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

                    item.TotalInvQuantityWithInvCorrection = subInventorySkuUsageReportItem.TotalInvQuantityWithInvCorrection;
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

        public async Task<InventoriesSummaryUsageReport> GetInventoriesSummaryUsageReportAsync(InventoriesSummaryUsageReportFilter filter)
        {
            InventoriesSummaryUsageReport toReturn = new InventoriesSummaryUsageReport();

            var categoryTree = new InventorySkuCategory();
            categoryTree.SubCategories = await _inventorySkuCategoryService.GetCategoriesTreeAsync(new InventorySkuCategoryTreeFilter());
            var data = await _sPEcommerceRepository.GetInventoriesSummaryUsageReportAsync(filter);
            var invIds = data.Select(p => p.IdInventorySku).Distinct().ToList();
            var inventories = await this.SelectAsync(invIds);

            List<InventoriesSummaryUsageDateItem> dates = new List<InventoriesSummaryUsageDateItem>();

            DateTime current = filter.From;
            current = new DateTime(current.Year, current.Month, 2, 0, 0, 0);
            dates.Add(new InventoriesSummaryUsageDateItem()
            {
                Date = current
            });
            while (current.AddMonths(1) < filter.To)
            {
                current = current.AddMonths(1);
                dates.Add(new InventoriesSummaryUsageDateItem()
                {
                    Date = current
                });
            }

            toReturn.TotalItems = dates.Select(p => new InventoriesSummaryUsageDateItem() { Date = p.Date, }).ToList();
            foreach (var inventoriesSummaryUsageRawReportItem in data)
            {
                inventoriesSummaryUsageRawReportItem.Date = inventoriesSummaryUsageRawReportItem.Date.AddDays(1);
            }

            foreach (var inventoriesSummaryUsageRawReportItem in data)
            {
                var inventory = inventories.FirstOrDefault(p => p.Id == inventoriesSummaryUsageRawReportItem.IdInventorySku);
                if (inventory == null)
                    continue;

                InventoriesSummaryUsageCategoryItem categoryItem = null;
                var category = FindUICategory(categoryTree, inventory.IdInventorySkuCategory);
                if (category != null)
                {
                    categoryItem = toReturn.Categories.FirstOrDefault(p => p.Id == category.Id);
                    if (categoryItem == null)
                    {
                        categoryItem = new InventoriesSummaryUsageCategoryItem()
                        {
                            Id = category.Id,
                            Name = category.Name,
                            TotalItems = dates.Select(p => new InventoriesSummaryUsageDateItem() { Date = p.Date, }).ToList(),
                        };
                        toReturn.Categories.Add(categoryItem);
                    }
                }
                else
                {
                    categoryItem = toReturn.Categories.FirstOrDefault(p => !p.Id.HasValue);
                    if (categoryItem == null)
                    {
                        categoryItem = new InventoriesSummaryUsageCategoryItem()
                        {
                            Id = null,
                            Name = "Not Specified",
                            TotalItems = dates.Select(p => new InventoriesSummaryUsageDateItem() { Date = p.Date, }).ToList(),
                        };
                        toReturn.Categories.Add(categoryItem);
                    }
                }

                InventoriesSummaryUsageInventoryItem inventoryItem = categoryItem.Inventories.FirstOrDefault(p => p.Id == inventoriesSummaryUsageRawReportItem.IdInventorySku);
                if (inventoryItem == null)
                {
                    inventoryItem = new InventoriesSummaryUsageInventoryItem()
                    {
                        Id = inventory.Id,
                        Code = inventory.Code,
                        Description = inventory.Description,
                        UnitOfMeasure = inventory.SafeData.UnitOfMeasure,
                        UnitOfMeasureAmount = inventory.SafeData.UnitOfMeasureAmount,
                        Items = dates.Select(p => new InventoriesSummaryUsageDateItem() { Date = p.Date, }).ToList(),
                    };
                    categoryItem.Inventories.Add(inventoryItem);
                }

                InventoriesSummaryUsageDateItem dateItem = inventoryItem.Items.FirstOrDefault(p => p.Date == inventoriesSummaryUsageRawReportItem.Date);
                if (dateItem != null)
                {
                    var quantity = inventoriesSummaryUsageRawReportItem.Quantity * inventory.SafeData.Quantity;
                    dateItem.Quantity += quantity;
                    inventoryItem.GrandTotal += quantity;
                }
            }

            foreach (var inventoriesSummaryUsageCategoryItem in toReturn.Categories)
            {
                foreach (var dateItem in inventoriesSummaryUsageCategoryItem.TotalItems)
                {
                    dateItem.Quantity = inventoriesSummaryUsageCategoryItem.Inventories.SelectMany(p => p.Items)
                            .Where(p => p.Date == dateItem.Date)
                            .Sum(p => p.Quantity);
                    inventoriesSummaryUsageCategoryItem.GrandTotal += dateItem.Quantity;
                }
            }

            foreach (var dateItem in toReturn.TotalItems)
            {
                dateItem.Quantity = toReturn.Categories.SelectMany(p => p.TotalItems).Where(p => p.Date == dateItem.Date).Sum(p => p.Quantity);
                toReturn.GrandTotal += dateItem.Quantity;
            }

            var notSpecifiedCategory = toReturn.Categories.FirstOrDefault(p => !p.Id.HasValue);
            if (notSpecifiedCategory != null)
            {
                toReturn.Categories.Remove(notSpecifiedCategory);
            }

            return toReturn;
        }

        public void ConvertInventoriesSummaryUsageReportForExport(InventoriesSummaryUsageReport report, out IList<DynamicExportColumn> columns, out IList<ExpandoObject> items)
        {
            columns = new List<DynamicExportColumn>();
            items = new List<ExpandoObject>();

            DynamicExportColumn column = new DynamicExportColumn();
            column.DisplayName = "Inventory Code";
            column.Name = "InventoryCode";
            columns.Add(column);

            column = new DynamicExportColumn();
            column.DisplayName = "Inventory Description";
            column.Name = "InventoryDescription";
            columns.Add(column);

            column = new DynamicExportColumn();
            column.DisplayName = "UOM";
            column.Name = "UOM";
            columns.Add(column);

            foreach (var inventoriesSummaryUsageDateItem in report.TotalItems)
            {
                column = new DynamicExportColumn();
                column.DisplayName = inventoriesSummaryUsageDateItem.Date.ToString("MMM-yy");
                column.Name = inventoriesSummaryUsageDateItem.Date.ToString("MMMyy");
                columns.Add(column);
            }

            column = new DynamicExportColumn();
            column.DisplayName = "Total";
            column.Name = "Total";
            columns.Add(column);

            dynamic item = null;
            IDictionary<string, object> map = null;
            foreach (var category in report.Categories)
            {
                foreach (var inventory in category.Inventories)
                {
                    item = new ExpandoObject();
                    map = (IDictionary<string, object>)item;
                    item.InventoryCode = inventory.Code;
                    item.InventoryDescription = inventory.Description;
                    item.UOM = inventory.UnitOfMeasureAmount;
                    foreach (var dateItem in inventory.Items)
                    {
                        map.Add(dateItem.Date.ToString("MMMyy"), dateItem.Quantity);
                    }
                    item.Total = inventory.GrandTotal;
                    items.Add((ExpandoObject)item);
                }

                item = new ExpandoObject();
                map = (IDictionary<string, object>)item;
                item.InventoryCode = category.Name;
                item.InventoryDescription = null;
                item.UOM = null;
                foreach (var dateItem in category.TotalItems)
                {
                    map.Add(dateItem.Date.ToString("MMMyy"), dateItem.Quantity);
                }
                item.Total = category.GrandTotal;
                items.Add((ExpandoObject)item);
            }


            item = new ExpandoObject();
            map = (IDictionary<string, object>)item;
            item.InventoryCode = "Total";
            item.InventoryDescription = null;
            item.UOM = null;
            foreach (var dateItem in report.TotalItems)
            {
                map.Add(dateItem.Date.ToString("MMMyy"), dateItem.Quantity);
            }
            item.Total = report.GrandTotal;
            items.Add((ExpandoObject)item);
        }

        #endregion

        #region Import

        public async Task<bool> ImportInventorySkusAsync(byte[] file,int userId)
        {
            List<InventorySkuImportItem> records = new List<InventorySkuImportItem>();
            Dictionary<string, ImportItemValidationGenericProperty> validationSettings = null;
            var recordType = typeof(InventorySkuImportItem);
            using (var memoryStream = new MemoryStream(file))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    CsvConfiguration configuration = new CsvConfiguration();
                    configuration.TrimFields = true;
                    configuration.TrimHeaders = true;
                    configuration.WillThrowOnMissingField = false;
                    configuration.RegisterClassMap<InventorySkuImportItemCsvMap>();
                    using (var csv = new CsvReader(streamReader, configuration))
                    {
                        PropertyInfo[] modelProperties = recordType.GetProperties();
                        validationSettings = BusinessHelper.GetAttrBaseImportValidationSettings(modelProperties);

                        int rowNumber = 1;
                        try
                        {
                            while (csv.Read())
                            {
                                InventorySkuImportItem item = (InventorySkuImportItem)csv.GetRecord(recordType);
                                item.RowNumber = rowNumber;
                                var localMessages = new List<MessageInfo>();
                                rowNumber++;

                                item.ErrorMessages = localMessages;
                                records.Add(item);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e.ToString());
                            throw new AppValidationException(e.Message);
                        }
                    }
                }
            }

            if (validationSettings != null)
            {
                BusinessHelper.ValidateAttrBaseImportItems(records, validationSettings);
            }

            List<string> codes = new List<string>();
            foreach (var inventorySkuImportItem in records)
            {
                int qty;
                if (!Int32.TryParse(inventorySkuImportItem.InvQty, out qty))
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Inv Qty", 
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], "Inv Qty")));
                }
                else
                {
                    inventorySkuImportItem.InvQtyInt = qty;
                }

                if (!Int32.TryParse(inventorySkuImportItem.UOMQty, out qty))
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("UOM Qty",
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], "UOM Qty")));
                }
                else
                {
                    inventorySkuImportItem.UOMQtyInt = qty;
                }

                decimal amount;
                if (!decimal.TryParse(inventorySkuImportItem.InvUnitAmt, out amount))
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Inv Unit Amt",
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], "Inv Unit Amt")));
                }
                else
                {
                    inventorySkuImportItem.InvUnitAmtDec = amount;
                }

                if (!codes.Contains(inventorySkuImportItem.Code))
                {
                    codes.Add(inventorySkuImportItem.Code);
                }
                else
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Code", ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuImportCodeDublicate]));
                }
            }

            //throw parsing and validation errors
            var messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var map = records.ToDictionary(p => p.Code, pp => pp);
            int i = 0;
            var groups = codes.GroupBy(x => i++ / SqlConstants.MAX_CONTAINS_COUNT).ToList();
            foreach (var group in groups)
            {
                var dbCodes = await ObjectRepository.Query(p => p.StatusCode != (int)RecordStatusCode.Deleted &&
                    group.Contains(p.Code)).SelectAsync(p => p.Code, false);
                foreach (var dbCode in dbCodes)
                {
                    InventorySkuImportItem item = null;
                    if (map.TryGetValue(dbCode, out item))
                    {
                        item.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Code",ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuImportCodeDublicate]));
                    }
                }
            }

            //codes dublicates in db
            messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var categories = await _inventorySkuCategoryService.GetCategoriesTreeAsync(new InventorySkuCategoryTreeFilter() {});
            var lookups = await _settingService.GetLookupsAsync(SettingConstants.INVENTORY_SKU_LOOKUP_NAMES.Split(','));
            if (lookups.Count != SettingConstants.INVENTORY_SKU_LOOKUP_NAMES.Split(',').Length)
            {
                throw new AppValidationException("Inventory lookups aren't configurated");
            }

            var productSourceLookup = lookups.FirstOrDefault(p=>p.Name==SettingConstants.INVENTORY_SKU_LOOKUP_PRODUCT_SOURCE_NAME);
            var unitOfMeasureLookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_UNIT_OF_MEASURE_NAME);
            var purchaseUnitOfMeasureLookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.INVENTORY_SKU_LOOKUP_PURCHASE_UNIT_OF_MEASURE_NAME);

            var toReturn = new List<InventorySkuDynamic>();
            foreach (var inventorySkuImportItem in records)
            {
                var item = Mapper.CreatePrototype();
                item.Code = inventorySkuImportItem.Code;
                item.StatusCode = string.Equals(inventorySkuImportItem.Active, "Y", StringComparison.InvariantCultureIgnoreCase)
                    ? (int) RecordStatusCode.Active
                    : (int) RecordStatusCode.NotActive;
                item.Description = inventorySkuImportItem.Description;
                item.IdEditedBy = userId;

                var idLookupVariant = productSourceLookup.LookupVariants.FirstOrDefault(p =>
                    string.Equals(p.ValueVariant, inventorySkuImportItem.Source, StringComparison.InvariantCultureIgnoreCase))?.Id;
                item.Data.ProductSource = idLookupVariant;
                if (!idLookupVariant.HasValue)
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Source",
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuLookupValueMissed], "Source")));
                }
                item.Data.Quantity = inventorySkuImportItem.InvQtyInt;

                idLookupVariant = unitOfMeasureLookup.LookupVariants.FirstOrDefault(p =>
                    string.Equals(p.ValueVariant, inventorySkuImportItem.InvUOM, StringComparison.InvariantCultureIgnoreCase))?.Id;
                item.Data.UnitOfMeasure = idLookupVariant;
                if (!idLookupVariant.HasValue)
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("InvUOM",
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuLookupValueMissed], "Inv UOM")));
                }
                item.Data.UnitOfMeasureAmount = inventorySkuImportItem.InvUnitAmtDec;

                idLookupVariant = purchaseUnitOfMeasureLookup.LookupVariants.FirstOrDefault(p =>
                    string.Equals(p.ValueVariant, inventorySkuImportItem.PurchaseUOM, StringComparison.InvariantCultureIgnoreCase))?.Id;
                item.Data.PurchaseUnitOfMeasure = idLookupVariant;
                if (!idLookupVariant.HasValue)
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("PurchaseUOM",
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuLookupValueMissed], "Purchase UOM")));
                }
                item.Data.PurchaseUnitOfMeasureAmount = inventorySkuImportItem.UOMQtyInt;
                
                var currentCategoryLevel = categories;
                var importCategoryRoute = inventorySkuImportItem.PartsCategory.Split('/');
                for (int j = 0; j < importCategoryRoute.Length; j++)
                {
                    var categoryName = importCategoryRoute[j];
                    var category = currentCategoryLevel.FirstOrDefault(p =>
                            string.Equals(p.Name, categoryName, StringComparison.InvariantCultureIgnoreCase));
                    if (category != null)
                    {
                        if (j == importCategoryRoute.Length - 1)
                        {
                            item.IdInventorySkuCategory = category.Id;
                        }
                        else
                        {
                            currentCategoryLevel = category.SubCategories ?? new List<InventorySkuCategory>();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (!item.IdInventorySkuCategory.HasValue)
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("PartsCategory",
                        ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuCategoryMissed]));
                }

                toReturn.Add(item);
            }

            //category and lookups errors
            messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            await InsertRangeAsync(toReturn);

            return true;
        }
        
        public async Task<bool> ImportSkuInventoryInfoAsync(byte[] file, int userId)
        {
            List<SkuInventoryInfoImportItem> records = new List<SkuInventoryInfoImportItem>();
            Dictionary<string, ImportItemValidationGenericProperty> validationSettings = null;
            var recordType = typeof(SkuInventoryInfoImportItem);
            using (var memoryStream = new MemoryStream(file))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    CsvConfiguration configuration = new CsvConfiguration();
                    configuration.TrimFields = true;
                    configuration.TrimHeaders = true;
                    configuration.WillThrowOnMissingField = false;
                    configuration.RegisterClassMap<SkuInventoryInfoImportItemCsvMap>();
                    using (var csv = new CsvReader(streamReader, configuration))
                    {
                        PropertyInfo[] modelProperties = recordType.GetProperties();
                        validationSettings = BusinessHelper.GetAttrBaseImportValidationSettings(modelProperties);

                        int rowNumber = 1;
                        try
                        {
                            while (csv.Read())
                            {
                                SkuInventoryInfoImportItem item = (SkuInventoryInfoImportItem)csv.GetRecord(recordType);
                                item.RowNumber = rowNumber;
                                item.ErrorMessages = new List<MessageInfo>();

                                var dateProperty = modelProperties.FirstOrDefault(p => p.Name == nameof(SkuInventoryInfoImportItem.BornDate));
                                var dateHeader = dateProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;

                                DateTime tDate;
                                var sDate = csv.GetField<string>(dateHeader);
                                if (!String.IsNullOrEmpty(sDate))
                                {
                                    if (DateTime.TryParse(sDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out tDate))
                                    {
                                        item.BornDate = TimeZoneInfo.ConvertTime(tDate, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);
                                    }
                                    else
                                    {
                                        item.ErrorMessages.Add(BusinessHelper.AddErrorMessage(dateHeader, 
                                            String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseDateError], dateHeader)));
                                    }
                                }
                                
                                rowNumber++;
                                records.Add(item);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogError(e.ToString());
                            throw new AppValidationException(e.Message);
                        }
                    }
                }
            }

            if (validationSettings != null)
            {
                BusinessHelper.ValidateAttrBaseImportItems(records, validationSettings);
            }

            //throw parsing and validation errors
            var messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var channelLookup = (await _settingService.GetLookupsAsync(new []{ SettingConstants.INVENTORY_SKU_LOOKUP_CHANNEL_NAME})).FirstOrDefault();

            List<string> codes = new List<string>();
            List<string> inventoryCodes = new List<string>();
            List<int> productIds = new List<int>();
            foreach (var inventorySkuImportItem in records)
            {
                bool assemble;
                if (!bool.TryParse(inventorySkuImportItem.Assemble, out assemble))
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Assemble",
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], "Assemble")));
                }
                else
                {
                    inventorySkuImportItem.AssembleBool = assemble;
                }

                var idLookupVariant = channelLookup.LookupVariants.FirstOrDefault(p =>
                    string.Equals(p.ValueVariant, inventorySkuImportItem.Channel, StringComparison.InvariantCultureIgnoreCase))?.Id;
                if (!idLookupVariant.HasValue)
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Channel",
                        string.Format(
                            ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuLookupValueMissed], "Channel")));
                }
                else
                {
                    inventorySkuImportItem.IdChannel = idLookupVariant.Value;
                }

                if (!codes.Contains(inventorySkuImportItem.SKU))
                {
                    codes.Add(inventorySkuImportItem.SKU);
                }
                else
                {
                    inventorySkuImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("SKU", 
                        ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuImportCodeDublicate]));
                }

                if (!string.IsNullOrEmpty(inventorySkuImportItem.Parts))
                {
                    var invCodes = inventorySkuImportItem.Parts.Split(',');
                    foreach (var invCode in invCodes)
                    {
                        var tInvCode = invCode.Trim();
                        tInvCode = QTYRegex.Replace(tInvCode, "");
                        if (!inventoryCodes.Contains(tInvCode))
                        {
                            inventoryCodes.Add(tInvCode);
                        }
                    }
                }
            }

            //throw parsing and validation errors
            messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }
            
            //get skus
            int i = 0;
            var groups = codes.GroupBy(x => i++ / SqlConstants.MAX_CONTAINS_COUNT).ToList();
            var skus = new Dictionary<string, Sku>(StringComparer.OrdinalIgnoreCase);
            foreach (var group in groups)
            {
                var tSkus = await _skuRepository.Query(p => p.StatusCode != (int)RecordStatusCode.Deleted && 
                    p.Product.StatusCode != (int)RecordStatusCode.Deleted &&
                    group.Contains(p.Code)).SelectAsync(false);
                skus.AddRange(tSkus.ToDictionary(p=>p.Code,pp=>pp, StringComparer.OrdinalIgnoreCase));
            }

            //get inv skus
            i = 0;
            var invGroups = inventoryCodes.GroupBy(x => i++ / SqlConstants.MAX_CONTAINS_COUNT).ToList();
            var invSkus = new Dictionary<string, InventorySku>(StringComparer.OrdinalIgnoreCase);
            foreach (var group in invGroups)
            {
                var tInvSkus = await ObjectRepository.Query(p => p.StatusCode != (int)RecordStatusCode.Deleted &&
                    group.Contains(p.Code)).SelectAsync(false);

                invSkus.AddRange(tInvSkus.ToDictionary(p => p.Code, pp => pp, StringComparer.OrdinalIgnoreCase));
            }

            foreach (var skuInventoryInfoImportItem in records)
            {
                Sku sku = null;
                if (skus.TryGetValue(skuInventoryInfoImportItem.SKU, out sku))
                {
                    skuInventoryInfoImportItem.IdSku = sku.Id;
                    skuInventoryInfoImportItem.IdProduct = sku.IdProduct;
                    if (productIds.All(p => p != skuInventoryInfoImportItem.IdProduct))
                    {
                        productIds.Add(skuInventoryInfoImportItem.IdProduct);
                    }
                }
                else
                {
                    skuInventoryInfoImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("SKU",
                        String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuNotFoundOrderImport], "SKU", skuInventoryInfoImportItem.SKU)));
                }

                if (!string.IsNullOrEmpty(skuInventoryInfoImportItem.Parts))
                {
                    var invCodes = skuInventoryInfoImportItem.Parts.Split(',');
                    foreach (var invCode in invCodes)
                    {
                        var tInvCode = invCode.Trim();
                        tInvCode = QTYRegex.Replace(tInvCode, "");
                        InventorySku invSku = null;
                        if (invSkus.TryGetValue(tInvCode, out invSku))
                        {
                            if (skuInventoryInfoImportItem.SkuToInventorySkus.All(p => p.IdInventorySku != invSku.Id))
                            {
                                int qty = 1;
                                var match = QTYRegex.Match(invCode);
                                if (match.Success)
                                {
                                    if (!Int32.TryParse(match.Value.Replace("(", "").Replace(")", ""),out qty))
                                    {
                                        skuInventoryInfoImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Parts",
                                            ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseIntError]));
                                    }
                                }
                                skuInventoryInfoImportItem.SkuToInventorySkus.Add(new SkuToInventorySku()
                                {
                                    IdInventorySku = invSku.Id,
                                    Quantity = qty
                                });
                            }
                        }
                        else
                        {
                            skuInventoryInfoImportItem.ErrorMessages.Add(BusinessHelper.AddErrorMessage("Parts",
                                String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InventorySkuNotFoundImport], invCode)));
                        }
                    }
                }
            }

            //skus and inventory skus exist in db
            messages = BusinessHelper.FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var products = await _productService.SelectTransfersAsync(productIds);
            foreach (var record in records)
            {
                var product = products.FirstOrDefault(p => p.ProductDynamic.Id == record.IdProduct);
                var sku = product.ProductDynamic.Skus.FirstOrDefault(p => p.Id == record.IdSku);
                sku.Data.InventorySkuChannel = record.IdChannel;
                sku.Data.Assemble = record.AssembleBool;
                if (record.BornDate.HasValue)
                {
                    sku.Data.BornDate = record.BornDate;
                }

                sku.SkusToInventorySkus = record.SkuToInventorySkus;
                product.ProductDynamic.IdEditedBy = userId;
            }

            await _productService.UpdateRangeAsync(products);

            return true;
        }

        #endregion
    }
}