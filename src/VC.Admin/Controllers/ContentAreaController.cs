using System;
using System.Collections.Generic;
using System.Linq;
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
	public class ContentAreaController:BaseApiController
    {
		private readonly IContentAreaService _contentAreaService;
	    private readonly ExtendedUserManager _userManager;

	    public ContentAreaController(IContentAreaService contentAreaService, ExtendedUserManager userManager)
	    {
	        _contentAreaService = contentAreaService;
	        _userManager = userManager;
	    }

	    [HttpGet]
		public async Task<Result<ContentAreaReadModel>> GetContentArea(int id)
		{
			var res = await _contentAreaService.GetContentAreaAsync(id);
			if (res == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

		    return new ContentAreaReadModel(res);
		}

		[HttpPost]
		public async Task<Result<ContentAreaReadModel>> UpdateContentArea([FromBody]ContentAreaUpdateModel model)
		{
			if (!Validate(model))
				return null;

			var contentArea = await _contentAreaService.GetContentAreaAsync(model.Id);
			if (contentArea == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			var context = HttpContext;
			contentArea.IdEditedBy = Convert.ToInt32(_userManager.GetUserId(context.User));
			contentArea.Updated = DateTime.Now;
			contentArea.Template = model.Template;

			await _contentAreaService.UpdateContentAreaAsync(contentArea);

		    return new ContentAreaReadModel(contentArea);
		}

	    [HttpGet]
	    public async Task<Result<IList<ContentAreaListItemModel>>> GetContentAreas()
	    {
			var result = await _contentAreaService.GetContentAreasAsync();

		    return result.Select(x => new ContentAreaListItemModel()
		    {
				Id = x.Id,
				Name = x.Name,
				Created = x.Created,
				Updated = x.Updated,
				AgentId = (x.User?.Profile != null) ? x.User.Profile.AgentId : string.Empty
		    }).ToList();
	    }
    }
}
