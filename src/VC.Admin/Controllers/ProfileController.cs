﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using Microsoft.AspNet.Hosting;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using VC.Admin.Models.Profile;
using VC.Admin.Validators.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Controllers
{
	[AdminAuthorize]
    public class ProfileController : BaseApiController
    {
	    private readonly IAdminUserService userService;
	    private readonly IHttpContextAccessor contextAccessor;

		public ProfileController(IAdminUserService userService, IHttpContextAccessor contextAccessor)
	    {
		    this.userService = userService;
			this.contextAccessor = contextAccessor;
        }

	    [HttpPost]
		[ControlMode(UpdateProfileMode.Default, typeof(UpdateProfileSettings))]
		public async Task<Result<GetProfileModel>> UpdateProfile([FromBody]UpdateProfileModel profileModel)
		{
			var context = contextAccessor.HttpContext;

		    if (context.User.Identity.IsAuthenticated)
		    {
		        var settings = new UpdateProfileSettings
		        {
		            Mode = (string.IsNullOrWhiteSpace(profileModel.OldPassword) &&
		                    string.IsNullOrWhiteSpace(profileModel.NewPassword) &&
		                    string.IsNullOrWhiteSpace(profileModel.ConfirmNewPassword))
		                ? UpdateProfileMode.Default
		                : UpdateProfileMode.WithPassword
		        };
		        if (!Validate(profileModel, settings))
					return null;

				var user = await userService.FindAsync(context.User.GetUserName());
				if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

			    var oldEmail = user.Email;

				user.FirstName = profileModel.FirstName;
				user.LastName = profileModel.LastName;
				user.Email = profileModel.Email;
			    user.UserName = profileModel.Email;

			    user = profileModel.Mode.Mode == UpdateProfileMode.WithPassword ? await userService.UpdateWithPasswordChangeAsync(user, profileModel.OldPassword, profileModel.NewPassword)
				    : await userService.UpdateAsync(user);

			    if (oldEmail != user.Email)
			    {
				    await userService.RefreshSignInAsync(user);
			    }

			    return new GetProfileModel()
			    {
					FirstName = user.FirstName,
					LastName = user.LastName,
					AgentId = user.Profile.AgentId,
					Email = user.Email
			    };
		    }

			return null;
		}

		[HttpGet]
	    public async Task<Result<GetProfileModel>> GetProfile()
		{
			var context = contextAccessor.HttpContext;

			if (context.User.Identity.IsAuthenticated)
			{
				var user = await userService.FindAsync(context.User.GetUserName());
				if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

				return new GetProfileModel()
				{
					FirstName = user.FirstName,
					LastName = user.LastName,
					AgentId = user.Profile.AgentId,
					Email = user.Email
				};
			}

			return null;
		}
	}
}