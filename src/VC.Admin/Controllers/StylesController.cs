using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.ContentManagement;
using VC.Admin.Models.UserManagement;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Content)]
	public class StylesController : BaseApiController
    {
		private readonly IStylesService _stylesService;

		public StylesController(IStylesService contentAreaService)
		{
			_stylesService = contentAreaService;
		}

		[HttpGet]
		public Result<StylesModel> GetStyles()
		{
			var res = _stylesService.GetStyles();
			if (res == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			return new StylesModel() { CSS = res};
		}

		[HttpPost]
		public async Task<Result<StylesModel>> UpdateStyles([FromBody]StylesModel model)
		{
			model.CSS = await _stylesService.UpdateStylesAsync(model.CSS);

			return model;
		}
    }
}
