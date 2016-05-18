using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services;
using VC.Admin.Models.Redirects;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Marketing)]
    public class RedirectController : BaseApiController
    {
        private readonly IRedirectService _redirectService;
        private readonly ExtendedUserManager _userManager;
        private readonly ILogger _logger;

        public RedirectController(IRedirectService redirectService, ILoggerProviderExtended loggerProvider, ExtendedUserManager userManager)
        {
            _redirectService = redirectService;
            _userManager = userManager;
            _logger = loggerProvider.CreateLogger<RedirectController>();
        }

        [HttpPost]
        public async Task<Result<PagedList<RedirectListItemModel>>> GetRedirects([FromBody]FilterBase filter)
        {
            var result = await _redirectService.GetRedirectsAsync(filter);

            var toReturn = new PagedList<RedirectListItemModel>
            {
                Items = result.Items.Select(p => new RedirectListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<RedirectManageModel>> GetRedirect(int id)
        {
            return new RedirectManageModel((await _redirectService.GetRedirectAsync(id)));
        }

        [HttpPost]
        public async Task<Result<RedirectManageModel>> UpdateRedirect([FromBody]RedirectManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();

            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            item = await _redirectService.UpdateRedirectAsync(item);

            return new RedirectManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteRedirect(int id, [FromBody] object model)
        {
            var sUserId = _userManager.GetUserId(User);
            int userId = Int32.Parse(sUserId);
            return await _redirectService.DeleteRedirectAsync(id, userId);
        }
    }
}