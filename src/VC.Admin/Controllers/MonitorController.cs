using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System.Collections.Generic;
using VC.Admin.Models.Setting;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.CatalogRequests;
using Microsoft.Net.Http.Headers;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using VC.Admin.Models.ContentManagement;
using VC.Admin.Models.EmailTemplates;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VC.Admin.Models.Settings;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Business.Services;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.Emails;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Profiling.Base;

namespace VC.Admin.Controllers
{
    public class MonitorController : BaseApiController
    {
        private readonly IAdminEditLockService _adminEditLockService;
        private readonly IOrderExport _exportService;

        public MonitorController(
            IAdminEditLockService adminEditLockService,
            IOrderExport exportService)
        {
            _adminEditLockService = adminEditLockService;
            _exportService = exportService;
        }

        #region EditLocks

        [HttpPost]
        [IgnoreBuildNumber]
        public Result<bool> EditLockPing([FromBody] EditLockPingModel model)
        {
            string browserUserAgent = Request.Headers["User-Agent"];

            return _adminEditLockService.AgentEditLockPing(model, browserUserAgent);
        }

        [HttpPost]
        [IgnoreBuildNumber]
        public Result<EditLockRequestModel> EditLockRequest([FromBody]EditLockRequestModel model)
        {
            string browserUserAgent = Request.Headers["User-Agent"];

            return _adminEditLockService.AgentEditLockRequest(model, browserUserAgent);
        }

        #endregion

        #region Export

        [AdminAuthorize(PermissionType.Orders)]
        [HttpGet]
        [IgnoreBuildNumber]
        public Result<OrderExportGeneralStatusModel> GetExportGeneralStatus()
        {
            return new OrderExportGeneralStatusModel
            {
                All = _exportService.TotalExporting,
                Exported = _exportService.TotalExported,
                Errors = _exportService.ExportErrors
            };
        }

        #endregion
    }
}