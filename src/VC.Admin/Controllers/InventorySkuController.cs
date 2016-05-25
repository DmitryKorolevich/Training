﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System;
using System.Dynamic;
using System.IO;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VC.Admin.Models.InventorySkus;
using VC.Admin.Models.Products;
using VitalChoice.Business.CsvExportMaps.Products;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Admin.Controllers
{
    public class InventorySkuController : BaseApiController
    {
        private readonly IInventorySkuCategoryService _inventorySkuCategoryService;
        private readonly IInventorySkuService _inventorySkuService;
        private readonly InventorySkuMapper _mapper;
        private readonly ISettingService _settingService;
        private readonly ICsvExportService<InventorySkuUsageReportItemForExport, InventorySkuUsageReportItemForExportCsvMap> _inventorySkuUsageReportItemForExportCSVExportService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ILogger _logger;
        private readonly ExtendedUserManager _userManager;

        public InventorySkuController(
            IInventorySkuCategoryService inventorySkuCategoryService,
            IInventorySkuService inventorySkuService,
            InventorySkuMapper mapper,
            ISettingService settingService,
            ICsvExportService<InventorySkuUsageReportItemForExport, InventorySkuUsageReportItemForExportCsvMap> inventorySkuUsageReportItemForExportCSVExportService,
            ILoggerProviderExtended loggerProvider, ExtendedUserManager userManager)
        {
            _inventorySkuCategoryService = inventorySkuCategoryService;
            _inventorySkuService = inventorySkuService;
            _mapper = mapper;
            _settingService = settingService;
            _inventorySkuUsageReportItemForExportCSVExportService = inventorySkuUsageReportItemForExportCSVExportService;
            _userManager = userManager;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            _logger = loggerProvider.CreateLogger<InventorySkuController>();
        }


        [HttpGet]
        public async Task<Result<IList<Lookup>>> GetInventorySkuLookups()
        {
            var result = await _settingService.GetLookupsAsync(SettingConstants.INVENTORY_SKU_LOOKUP_NAMES.Split(','));
            result.ForEach(p =>
            {
                if (p.LookupVariants != null)
                {
                    foreach (var lookupVariant in p.LookupVariants)
                    {
                        lookupVariant.Lookup = null;
                    }
                }
            });

            return result.ToList();
        }

        #region InventorySkus

        [HttpPost]
        public async Task<Result<PagedList<InventorySkuListItemModel>>> GetInventorySkus([FromBody]InventorySkuFilter filter)
        {
            var toReturn = await _inventorySkuService.GetInventorySkusAsync(filter);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<InventorySkuListItemModel>> GetShortInventorySku([FromBody]InventorySkuFilter filter)
        {
            var result = await _inventorySkuService.GetInventorySkusAsync(filter);

            return result.Items.FirstOrDefault();
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuManageModel>> GetInventorySku(int id)
        {
            if (id == 0)
            {
                return new InventorySkuManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                };
            }

            var item = await _inventorySkuService.SelectAsync(id);
            if (item == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }
            InventorySkuManageModel toReturn = await _mapper.ToModelAsync<InventorySkuManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuManageModel>> UpdateInventorySku([FromBody]InventorySkuManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = await _mapper.FromModelAsync(model);

            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            if (item.Id > 0)
            {
                item = await _inventorySkuService.UpdateAsync(item);
            }
            else
            {
                item = await _inventorySkuService.InsertAsync(item);
            }

            var toReturn = await _mapper.ToModelAsync<InventorySkuManageModel> (item);

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<bool>> DeleteInventorySku(int id)
        {
            return await _inventorySkuService.DeleteAsync(id);
        }

        #endregion

        #region InventorySkuCategories

        [HttpPost]
        public async Task<Result<IList<InventorySkuCategoryTreeItemModel>>> GetInventorySkuCategoriesTree([FromBody]InventorySkuCategoryTreeFilter filter)
        {
            var result = await _inventorySkuCategoryService.GetCategoriesTreeAsync(filter);

            return result.Select(p => new InventorySkuCategoryTreeItemModel(p)).ToList();
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<bool>> UpdateInventorySkuCategoriesTree([FromBody]IList<InventorySkuCategoryTreeItemModel> model)
        {
            IList<InventorySkuCategory> categories = new List<InventorySkuCategory>();
            if (model != null)
            {
                foreach (var modelCategory in model)
                {
                    categories.Add(modelCategory.Convert());
                }
            }

            return await _inventorySkuCategoryService.UpdateCategoriesTreeAsync(categories);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuCategoryManageModel>> GetInventorySkuCategory(int id)
        {
            if (id == 0)
            {
                return new InventorySkuCategoryManageModel()
                {
                };
            }
            return new InventorySkuCategoryManageModel((await _inventorySkuCategoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<InventorySkuCategoryManageModel>> UpdateInventorySkuCategory([FromBody]InventorySkuCategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await _inventorySkuCategoryService.UpdateCategoryAsync(item);

            return new InventorySkuCategoryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.InventorySkus)]
        public async Task<Result<bool>> DeleteInventorySkuCategory(int id)
        {
            return await _inventorySkuCategoryService.DeleteCategoryAsync(id);
        }

        #endregion

        #region Reports

        [HttpPost]
        [AdminAuthorize(PermissionType.Reports)]
        public async Task<Result<ICollection<InventorySkuUsageReportItem>>> GetInventorySkuUsageReport([FromBody]InventorySkuUsageReportFilter filter)
        {
            filter.To = filter.To.AddDays(1);

            var toReturn = await _inventorySkuService.GetInventorySkuUsageReportAsync(filter);

            return toReturn.ToList();
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Reports)]
        public async Task<FileResult> GetInventorySkuUsageReportFile([FromQuery]string from, [FromQuery]string to,
             [FromQuery]string skuids, [FromQuery]string invskuids)
        {
            var dFrom = from.GetDateFromQueryStringInPst(_pstTimeZoneInfo);
            var dTo = to.GetDateFromQueryStringInPst(_pstTimeZoneInfo);
            if (!dFrom.HasValue || !dTo.HasValue)
            {
                return null;
            }

            InventorySkuUsageReportFilter filter = new InventorySkuUsageReportFilter()
            {
                From = dFrom.Value,
                To = dTo.Value,
                SkuIds =!string.IsNullOrEmpty(skuids) ? skuids.Split(',').Select(Int32.Parse).ToList() : new List<int>(),
                InvSkuIds = !string.IsNullOrEmpty(invskuids) ? invskuids.Split(',').Select(Int32.Parse).ToList() : new List<int>(),
            };

            filter.To = filter.To.AddDays(1);

            var result = await _inventorySkuService.GetInventorySkuUsageReportForExportAsync(filter);

            var data = _inventorySkuUsageReportItemForExportCSVExportService.ExportToCsv(result);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.INVENTORY_SKUS_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Reports)]
        public async Task<Result<InventoriesSummaryUsageReport>> GetInventoriesSummaryUsageReport([FromBody]InventoriesSummaryUsageReportFilter filter)
        {
            filter.To = filter.To.AddDays(1);
            var toReturn = await _inventorySkuService.GetInventoriesSummaryUsageReportAsync(filter);

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Reports)]
        public async Task<FileResult> GetInventoriesSummaryUsageReportFile([FromQuery]string from, [FromQuery]string to,
             [FromQuery]string sku, [FromQuery]string invsku, [FromQuery]bool? assemble, [FromQuery]string idsinvcat)
        {
            var dFrom = from.GetDateFromQueryStringInPst(_pstTimeZoneInfo);
            var dTo = to.GetDateFromQueryStringInPst(_pstTimeZoneInfo);
            if (!dFrom.HasValue || !dTo.HasValue)
            {
                return null;
            }

            InventoriesSummaryUsageReportFilter filter = new InventoriesSummaryUsageReportFilter()
            {
                From = dFrom.Value,
                To = dTo.Value,
                Sku = sku,
                InvSku = invsku,
                Assemble = assemble,
                IdsInvCat = !string.IsNullOrEmpty(idsinvcat) ? idsinvcat.Split(',').Select(Int32.Parse).ToList() : null,
            };

            filter.To = filter.To.AddDays(1);

            var result = await _inventorySkuService.GetInventoriesSummaryUsageReportAsync(filter);
            IList<DynamicExportColumn> columns = null;
            IList<ExpandoObject> items = null;
            _inventorySkuService.ConvertInventoriesSummaryUsageReportForExport(result, out columns, out items);

            var data = CsvExportService.ExportToCsv(columns, items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.INVENTORY_SUMMARY_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        #endregion

    }
}