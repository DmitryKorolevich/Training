using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services;
using System;
using Microsoft.Net.Http.Headers;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Infrastructure.Domain.Transfer.VitalGreen;
using VitalChoice.Interfaces.Services.Orders;

namespace VC.Admin.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IOrderReportService _orderReportService;
        private readonly ILogger _logger;

        public ReportController(
            IOrderReportService orderReportService,
            ILoggerProviderExtended loggerProvider)
        {
            _orderReportService = orderReportService;
            this._logger = loggerProvider.CreateLogger<ReportController>();
        }

        [HttpGet]
        public async Task<IActionResult> KPI()
        {
            //var model = await _orderReportService.GetKPIReportAsync();
            var model = await _orderReportService.CreateKPIReportAsync();

            return View(model);
        }
    }
}