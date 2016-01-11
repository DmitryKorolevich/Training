using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using System;
using VC.Admin.Models;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Business.CsvExportMaps;
using Microsoft.Net.Http.Headers;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Marketing)]
    public class GCController : BaseApiController
    {
        private readonly IGcService GCService;
        private readonly ICsvExportService<GCWithOrderListItemModel, GCWithOrderListItemModelCsvMap> _gCWithOrderListItemModelCsvMapCSVExportService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ILogger logger;

        public GCController(IGcService GCService,
            ICsvExportService<GCWithOrderListItemModel, GCWithOrderListItemModelCsvMap> gCWithOrderListItemModelCsvMapCSVExportService,
            ILoggerProviderExtended loggerProvider)
        {
            this.GCService = GCService;
            _gCWithOrderListItemModelCsvMapCSVExportService = gCWithOrderListItemModelCsvMapCSVExportService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpPost]
        public async Task<Result<PagedList<GCListItemModel>>> GetGiftCertificates([FromBody]GCFilter filter)
        {
            var result = await GCService.GetGiftCertificatesAsync(filter);

            var toReturn = new PagedList<GCListItemModel>
            {
                Items = result.Items.Select(p => new GCListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<GCStatisticModel>> GetGiftCertificatesWithOrderInfo([FromBody]GCFilter filter)
        {
            GCStatisticModel toReturn = await GCService.GetGiftCertificatesWithOrderInfoAsync(filter);

            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetGiftCertificatesWithOrderInfoReportFile([FromQuery]DateTime from, [FromQuery]DateTime to,
            [FromQuery]int? type = null, [FromQuery]int? status = null, [FromQuery]string billinglastname = null, [FromQuery]string shippinglastname = null)
        {
            GCFilter filter = new GCFilter()
            {
                From = from,
                To = to,
                Type = type.HasValue ? (GCType?)type.Value : null,
                StatusCode = status.HasValue ? (RecordStatusCode?)status.Value : null,
                BillingAddress = !String.IsNullOrEmpty(billinglastname) ? new CustomerAddressFilter() { LastName= billinglastname }:
                    null,
                ShippingAddress = !String.IsNullOrEmpty(shippinglastname) ? new CustomerAddressFilter() { LastName = shippinglastname } :
                    null,
            };
            filter.Paging = null;

            var data = await GCService.GetGiftCertificatesWithOrderInfoAsync(filter);
            foreach (var item in data.Items)
            {
                item.Created = TimeZoneInfo.ConvertTime(item.Created, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }

            var result = _gCWithOrderListItemModelCsvMapCSVExportService.ExportToCsv(data.Items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.GIFT_CERTIFICATES_REPORT_STATISTIC, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(result, "text/csv");
        }

        [HttpGet]
        public async Task<Result<GCManageModel>> GetGiftCertificate(int id)
        {
            return new GCManageModel((await GCService.GetGiftCertificateAsync(id)));
        }

        [HttpPost]
        public async Task<Result<GCAddingModel>> GetGiftCertificatesAdding([FromBody] object model)
        {
            var item = new GCAddingModel();
            item.Quantity = 1;
            item.Balance = 0;

            return await Task.FromResult<GCAddingModel>(item);
        }

        [HttpPost]
        public async Task<Result<ICollection<GCListItemModel>>> AddGiftCertificates(int quantity, [FromBody]GCManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            return (await GCService.AddGiftCertificatesAsync(quantity, item)).Select(p => new GCListItemModel(p)).ToList();
        }

        [HttpPost]
        public async Task<Result<GCManageModel>> UpdateGiftCertificate([FromBody]GCManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await GCService.UpdateGiftCertificateAsync(item);

            return new GCManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> SendGiftCertificateEmail([FromBody]GCEmailModel model)
        {
            if (!Validate(model))
                return false;
            var item = model.Convert();
            return await GCService.SendGiftCertificateEmailAsync(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteGiftCertificate(int id, [FromBody] object model)
        {
            return await GCService.DeleteGiftCertificateAsync(id);
        }
    }
}