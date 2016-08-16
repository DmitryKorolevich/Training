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

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Reports)]
    public class VitalGreenController : BaseApiController
    {
        private readonly IVitalGreenService _vitalGreenService;
        private readonly ICsvExportService<VitalGreenRequest, VitalGreenRequestCsvMap> _csvExportVitalGreenRequestService;
        private readonly ILogger _logger;

        public VitalGreenController(
            IVitalGreenService vitalGreenService,
            ICsvExportService<VitalGreenRequest, VitalGreenRequestCsvMap> csvExportVitalGreenRequestService,
            ILoggerFactory loggerProvider)
        {
            _vitalGreenService = vitalGreenService;
            _csvExportVitalGreenRequestService = csvExportVitalGreenRequestService;
            this._logger = loggerProvider.CreateLogger<VitalGreenController>();
        }

        [HttpPost]
        public async Task<Result<VitalGreenReportModel>> GetVitalGreenReport([FromBody]VitalGreenReportFilter filter)
        {
            DateTime utcDate = new DateTime(filter.Year,filter.Month,1);
            utcDate = TimeZoneInfo.ConvertTime(utcDate, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);

            var toReturn = await _vitalGreenService.GetVitalGreenReport(utcDate);
            toReturn.Year = filter.Year;
            toReturn.Month = filter.Month;
            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetVitalGreenReportFile([FromQuery]string year, [FromQuery]string month)
        {
            DateTime utcDate = new DateTime(Int32.Parse(year), Int32.Parse(month), 1);
            utcDate = TimeZoneInfo.ConvertTime(utcDate, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);

            var toReturn = await _vitalGreenService.GetVitalGreenReport(utcDate);
            var requests = new List<VitalGreenRequest>();
            foreach(var date in toReturn.Dates)
            {
                requests.AddRange(date.Requests);
            }
            var data = _csvExportVitalGreenRequestService.ExportToCsv(requests);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.VITAL_GREEN_REPORT_FILE_NAME_FORMAT, month, year),
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }
    }
}