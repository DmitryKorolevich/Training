using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Content)]
	public class StylesController : BaseApiController
    {
		private readonly IStylesService _stylesService;
        private readonly ExtendedUserManager _userManager;

        public StylesController(IStylesService contentAreaService, ExtendedUserManager userManager)
        {
            _stylesService = contentAreaService;
            _userManager = userManager;
        }

	    [HttpGet]
		public async Task<Result<StylesModel>> GetStyles()
		{
			var res = await _stylesService.GetStyles();
			if (res == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			return new StylesModel() { CSS = res.Styles};
		}

		[HttpPost]
		public async Task<Result<StylesModel>> UpdateStyles([FromBody]StylesModel model)
		{
			var res = await _stylesService.GetStyles();
			if (res == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			res.IdEditedBy = Convert.ToInt32(_userManager.GetUserId(User));
			res.Updated = DateTime.Now;
			res.Styles = model.CSS;

			res = await _stylesService.UpdateStylesAsync(res);
			model.CSS = res.Styles;

			return model;
		}
    }
}
