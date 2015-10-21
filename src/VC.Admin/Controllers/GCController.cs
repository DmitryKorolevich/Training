using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities.Permissions;
using System.Security.Claims;
using System;
using VC.Admin.Models;
using VitalChoice.Business.Services;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Marketing)]
    public class GCController : BaseApiController
    {
        private readonly IGcService GCService;
        private readonly ILogger logger;

        public GCController(IGcService GCService, ILoggerProviderExtended loggerProvider)
        {
            this.GCService = GCService;
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