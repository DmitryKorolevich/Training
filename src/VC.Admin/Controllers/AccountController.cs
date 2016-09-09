using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.Account;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AllowAnonymous]
    public class AccountController : BaseApiController
    {
	    private readonly IAdminUserService _userService;
	    private readonly ExtendedUserManager _userManager;

	    public AccountController(IAdminUserService userService, ExtendedUserManager userManager)
	    {
	        this._userService = userService;
	        _userManager = userManager;
	    }

	    private async Task<UserInfoModel> PopulateUserInfoModel(ApplicationUser user)
		{
			return new UserInfoModel()
			{
                Id = user.Id,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				IsSuperAdmin = await _userService.IsSuperAdmin(user),
				Permissions = await _userService.GetUserPermissions(user)
			};
		}

		[HttpGet]
	    public async Task<Result<ActivateUserModel>> GetUser(Guid id)
		{
			var result = await _userService.GetByTokenAsync(id);
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

			var user = await _userService.GetAsync(model.PublicId);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.IsConfirmed = true;
			user.Status = UserStatus.Active;
			user.ConfirmationToken = Guid.Empty;

			await _userService.UpdateAsync(user, null, model.Password);

			await _userService.SignInAsync(user);

			return await PopulateUserInfoModel(user);
		}

		[HttpPost]
		public async Task<Result<UserInfoModel>> ResetPassword([FromBody]ResetPasswordModel model)
		{
			if (!Validate(model))
				return null;

		    var id = await _userService.GetIdByEmailAsync(model.Email);
            ApplicationUser user = null;

            if (id.HasValue)
		    {
		        user = await _userService.ResetPasswordAsync(id.Value, model.Token, model.Password);
		    }
		    if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			await _userService.SignInAsync(user);

			return await PopulateUserInfoModel(user);
		}

		[HttpPost]
		public async Task<Result<UserInfoModel>> Login([FromBody]LoginModel model)
		{
			if (!Validate(model))
				return null;

            var id = await _userService.GetIdByEmailAsync(model.Email);
            ApplicationUser user = null;

		    if (id.HasValue)
		    {
		        user = await _userService.SignInAsync(id.Value, model.Password);
		    }
		    if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.IncorrectUserPassword]);
			}

			return await PopulateUserInfoModel(user);
		}

		[HttpGet]
		public async Task<Result<UserInfoModel>> GetCurrentUser()
		{
			if (User.Identity.IsAuthenticated)
			{
			    int id;
                ApplicationUser user = null;
                if (int.TryParse(_userManager.GetUserId(User), out id))
			    {
			        user = await _userService.FindAsync(id);
			    }
			    if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

				return await PopulateUserInfoModel(user);
			}

			return null;
		}

		[HttpPost]
		public async Task<bool> Logout()
		{
			var context = HttpContext;

			if (context.User.Identity.IsAuthenticated)
			{
                int id;
                ApplicationUser user = null;

                if (int.TryParse(_userManager.GetUserId(User), out id))
                {
                    user = await _userService.FindAsync(id);
                }
                if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

				await _userService.SignOutAsync(user);
			}

			return true;
		}
    }
}