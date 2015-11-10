using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Account;
using VitalChoice.Core.Base;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AllowAnonymous]
    public class AccountController : BaseApiController
    {
	    private readonly IAdminUserService userService;
		private readonly IHttpContextAccessor contextAccessor;

		public AccountController(IAdminUserService userService, IHttpContextAccessor contextAccessor)
		{
			this.userService = userService;
			this.contextAccessor = contextAccessor;
		}

		private async Task<UserInfoModel> PopulateUserInfoModel(ApplicationUser user)
		{
			return new UserInfoModel()
			{
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				IsSuperAdmin = await userService.IsSuperAdmin(user),
				Permissions = await userService.GetUserPermissions(user)
			};
		}

		[HttpGet]
	    public async Task<Result<ActivateUserModel>> GetUser(Guid id)
		{
			var result = await userService.GetByTokenAsync(id);
			if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
			}
			if (result.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
			}

			return new ActivateUserModel()
			{
				AgentId = result.Profile.AgentId,
				FirstName = result.FirstName,
				LastName = result.LastName,
				Email = result.Email,
				PublicId = result.PublicId
			};
		}

		[HttpPost]
		public async Task<Result<UserInfoModel>> Activate([FromBody]CreateAccountModel model)
		{
			if (!Validate(model))
				return null;

			var user = await userService.GetAsync(model.PublicId);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.IsConfirmed = true;
			user.Status = UserStatus.Active;
			user.ConfirmationToken = Guid.Empty;

			await userService.UpdateAsync(user, null, model.Password);

			await userService.SignInAsync(user);

			return await PopulateUserInfoModel(user);
		}

		[HttpPost]
		public async Task<Result<UserInfoModel>> ResetPassword([FromBody]ResetPasswordModel model)
		{
			if (!Validate(model))
				return null;

			await userService.ResetPasswordAsync(model.Email, model.Token, model.Password);

			var user = await userService.FindAsync(model.Email);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			await userService.SignInAsync(user);

			return await PopulateUserInfoModel(user);
		}

		[HttpPost]
		public async Task<Result<UserInfoModel>> Login([FromBody]LoginModel model)
		{
			if (!Validate(model))
				return null;

			var user = await userService.SignInAsync(model.Email, model.Password);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
			}

			return await PopulateUserInfoModel(user);
		}

		[HttpGet]
		public async Task<Result<UserInfoModel>> GetCurrentUser()
		{
			var context = contextAccessor.HttpContext;

			if (context.User.Identity.IsAuthenticated)
			{
				var user = await userService.FindAsync(context.User.GetUserName());
				if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

				return await PopulateUserInfoModel(user);
			}

			return null;
		}

		[HttpPost]
		public async Task<bool> Logout([FromBody] object model)
		{
			var context = contextAccessor.HttpContext;

			if (context.User.Identity.IsAuthenticated)
			{
				var user = await userService.FindAsync(context.User.GetUserName());
				if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

				await userService.SignOutAsync(user);
			}

			return true;
		}
    }
}