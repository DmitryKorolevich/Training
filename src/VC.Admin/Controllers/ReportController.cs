﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services;
using System;
using Microsoft.Net.Http.Headers;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Business.CsvExportMaps.Orders;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.VitalGreen;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Infrastructure.Domain;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IOrderReportService _orderReportService;
        private readonly ILogger _logger;

        public ReportController(
            IOrderReportService orderReportService,
            ILoggerFactory loggerProvider)
        {
            _orderReportService = orderReportService;
            this._logger = loggerProvider.CreateLogger<ReportController>();
        }

        [HttpGet]
        public async Task<IActionResult> KPI()
        {
            var model = await _orderReportService.GetKPIReportAsync();
            return View(model);
        }
    }
}