using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
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
using VC.Admin.Models.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain;

namespace VC.Admin.Controllers
{
    public class GCController : BaseApiController
    {
        private readonly IGcService GCService;
        private readonly IOrderSchedulerService OrderSchedulerService;
        private readonly ICsvExportService<GCWithOrderListItemModel, GcWithOrderListItemModelCsvMap> _gCWithOrderListItemModelCsvMapCSVExportService;
        private readonly ExtendedUserManager _userManager;
        private readonly ILogger logger;

        public GCController(IGcService GCService,
            IOrderSchedulerService OrderSchedulerService,
            ICsvExportService<GCWithOrderListItemModel, GcWithOrderListItemModelCsvMap> gCWithOrderListItemModelCsvMapCSVExportService,
            ILoggerFactory loggerProvider, ExtendedUserManager userManager)
        {
            this.GCService = GCService;
            this.OrderSchedulerService = OrderSchedulerService;
            _gCWithOrderListItemModelCsvMapCSVExportService = gCWithOrderListItemModelCsvMapCSVExportService;
            _userManager = userManager;
            this.logger = loggerProvider.CreateLogger<GCController>();
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

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<GCStatisticModel>> GetGiftCertificatesWithOrderInfo([FromBody]GCFilter filter)
        {
            filter.To = filter.To?.AddDays(1) ?? filter.To;
            GCStatisticModel toReturn = await GCService.GetGiftCertificatesWithOrderInfoAsync(filter);

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<FileResult> GetGiftCertificatesWithOrderInfoReportFile([FromQuery]string from, [FromQuery]string to,
            [FromQuery]int? type = null, [FromQuery]int? status = null, [FromQuery]string billinglastname = null, [FromQuery]string shippinglastname = null)
        {
            DateTime? dFrom = !string.IsNullOrEmpty(from) ? from.GetDateFromQueryStringInPst(TimeZoneHelper.PstTimeZoneInfo) : null;
            DateTime? dTo = !string.IsNullOrEmpty(to) ? to.GetDateFromQueryStringInPst(TimeZoneHelper.PstTimeZoneInfo) : null;

            GCFilter filter = new GCFilter()
            {
                From = dFrom,
                To = dTo?.AddDays(1) ?? dTo,
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
                item.Created = TimeZoneInfo.ConvertTime(item.Created, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
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
        public async Task<Result<GCManageModel>> GetGiftCertificate(string id)
        {
            int idGC = 0;
            if (id != null && !Int32.TryParse(id, out idGC))
                throw new NotFoundException();

            var item = await GCService.GetGiftCertificateAsync(idGC);
            if (item == null)
                throw new NotFoundException();

            return new GCManageModel(item);
        }

        [AdminAuthorize(PermissionType.Marketing)]
        [HttpPost]
        public async Task<Result<GCAddingModel>> GetGiftCertificatesAdding()
        {
            var item = new GCAddingModel();
            item.Quantity = 1;
            item.Balance = 0;

            return await Task.FromResult<GCAddingModel>(item);
        }

        [AdminAuthorize(PermissionType.Marketing)]
        [HttpPost]
        public async Task<Result<ICollection<GCListItemModel>>> AddGiftCertificates(int quantity, [FromBody]GCManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            return (await GCService.AddManualGiftCertificatesAsync(quantity, item)).Select(p => new GCListItemModel(p)).ToList();
        }

        [AdminAuthorize(PermissionType.Marketing)]
        [HttpPost]
        public async Task<Result<GCManageModel>> UpdateGiftCertificate([FromBody]GCManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await GCService.UpdateGiftCertificateAsync(item);

            return new GCManageModel(item);
        }

        [AdminAuthorize(PermissionType.Marketing)]
        [HttpPost]
        public async Task<Result<bool>> SendGiftCertificateEmail([FromBody]GCEmailModel model)
        {
            if (!Validate(model))
                return false;
            var item = model.Convert();
            return await GCService.SendGiftCertificateEmailAsync(item);
        }

        [AdminAuthorize(PermissionType.Marketing)]
        [HttpPost]
        public async Task<Result<bool>> DeleteGiftCertificate(int id)
        {
            return await GCService.DeleteGiftCertificateAsync(id);
        }
    }
}