using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
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

			var context = HttpContext;
			res.IdEditedBy = Convert.ToInt32(context.User.GetUserId());
			res.Updated = DateTime.Now;
			res.Styles = model.CSS;

			res = await _stylesService.UpdateStylesAsync(res);
			model.CSS = res.Styles;

			return model;
		}
    }
}
