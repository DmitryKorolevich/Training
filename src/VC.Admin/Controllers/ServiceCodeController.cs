using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using VC.Admin.Models.Orders;
using VitalChoice.Business.CsvExportMaps.Orders;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    public class ServiceCodeController : BaseApiController
    {
        private readonly IServiceCodeService _serviceCodeService;
        private readonly ICsvExportService<ServiceCodeRefundItem, ServiceCodeRefundItemCsvMap> _serviceCodeRefundItemCSVExportService;
        private readonly ICsvExportService<ServiceCodeReshipItem, ServiceCodeReshipItemCsvMap> _serviceCodeReshipItemCSVExportService;

        public ServiceCodeController(IServiceCodeService serviceCodeService,
            ICsvExportService<ServiceCodeRefundItem, ServiceCodeRefundItemCsvMap> serviceCodeRefundItemCSVExportService,
            ICsvExportService<ServiceCodeReshipItem, ServiceCodeReshipItemCsvMap> serviceCodeReshipItemCSVExportService)
        {
            _serviceCodeService = serviceCodeService;
            _serviceCodeRefundItemCSVExportService = serviceCodeRefundItemCSVExportService;
            _serviceCodeReshipItemCSVExportService = serviceCodeReshipItemCSVExportService;
        }

        [HttpPost]
        public async Task<Result<ServiceCodesReport>> GetServiceCodesReport([FromBody]ServiceCodesReportFilter filter)
        {
            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }
            return await _serviceCodeService.GetServiceCodesReportAsync(filter);
        }

        [HttpPost]
        public async Task<Result<PagedList<ServiceCodeRefundItem>>> GetServiceCodeRefundItems([FromBody]ServiceCodeItemsFilter filter)
        {
            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }

            var toReturn = await _serviceCodeService.GetServiceCodeRefundItemsAsync(filter);

            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetServiceCodeRefundItemsReportFile([FromQuery]DateTime? from, [FromQuery]DateTime? to,
             [FromQuery]int servicecode)
        {
            ServiceCodeItemsFilter filter = new ServiceCodeItemsFilter()
            {
                From = from,
                To = to,
                ServiceCode = servicecode,
            };

            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }

            var result = await _serviceCodeService.GetServiceCodeRefundItemsAsync(filter);

            var data = _serviceCodeRefundItemCSVExportService.ExportToCsv(result.Items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.REFUNDS_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [HttpPost]
        public async Task<Result<PagedList<ServiceCodeReshipItem>>> GetServiceCodeReshipItems([FromBody]ServiceCodeItemsFilter filter)
        {
            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }

            var toReturn = await _serviceCodeService.GetServiceCodeReshipItemsAsync(filter);

            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetServiceCodeReshipItemsReportFile([FromQuery]DateTime? from, [FromQuery]DateTime? to,
            [FromQuery]int servicecode)
        {
            ServiceCodeItemsFilter filter = new ServiceCodeItemsFilter()
            {
                From = from,
                To = to,
                ServiceCode = servicecode,
            };

            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }

            var result = await _serviceCodeService.GetServiceCodeReshipItemsAsync(filter);

            var data = _serviceCodeReshipItemCSVExportService.ExportToCsv(result.Items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.RESHIPS_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [HttpPost]
        public async Task<Result<bool>> AssignServiceCodeForRefunds([FromBody]ICollection<int> ids, [FromQuery]int servicecode)
        {
            var toReturn = await _serviceCodeService.AssignServiceCodeForRefundsAsync(ids,servicecode);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> AssignServiceCodeForReships([FromBody]ICollection<int> ids, [FromQuery]int servicecode)
        {
            var toReturn = await _serviceCodeService.AssignServiceCodeForReshipsAsync(ids, servicecode);

            return toReturn;
        }
    }
}
