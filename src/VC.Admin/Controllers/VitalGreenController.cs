using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services;
using System;
using VitalChoice.Business.ExportMaps;
using Microsoft.Net.Http.Headers;
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
        private readonly IExportService<VitalGreenRequest, VitalGreenRequestCsvMap> _exportVitalGreenRequestService;
        private readonly ILogger _logger;
        private readonly TimeZoneInfo _pstTimeZoneInfo;

        public VitalGreenController(
            IVitalGreenService vitalGreenService,
            IExportService<VitalGreenRequest, VitalGreenRequestCsvMap> exportVitalGreenRequestService,
            ILoggerProviderExtended loggerProvider)
        {
            _vitalGreenService = vitalGreenService;
            _exportVitalGreenRequestService = exportVitalGreenRequestService;
            this._logger = loggerProvider.CreateLoggerDefault();
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        [HttpPost]
        public async Task<Result<VitalGreenReportModel>> GetVitalGreenReport([FromBody]VitalGreenReportFilter filter)
        {
            DateTime utcDate = new DateTime(filter.Year,filter.Month,1);
            utcDate = TimeZoneInfo.ConvertTime(utcDate, _pstTimeZoneInfo, TimeZoneInfo.Local);

            var toReturn = await _vitalGreenService.GetVitalGreenReport(utcDate);
            toReturn.Year = filter.Year;
            toReturn.Month = filter.Month;
            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetVitalGreenReportFile([FromQuery]string year, [FromQuery]string month)
        {
            DateTime utcDate = new DateTime(Int32.Parse(year), Int32.Parse(month), 1);
            utcDate = TimeZoneInfo.ConvertTime(utcDate, _pstTimeZoneInfo, TimeZoneInfo.Local);

            var toReturn = await _vitalGreenService.GetVitalGreenReport(utcDate);
            var requests = new List<VitalGreenRequest>();
            foreach(var date in toReturn.Dates)
            {
                requests.AddRange(date.Requests);
            }
            var data = _exportVitalGreenRequestService.ExportToCSV(requests);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.VITAL_GREEN_REPORT_FILE_NAME_FORMAT, month, year),
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }
    }
}