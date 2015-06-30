using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
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
using VC.Admin.Models.UserManagement;
using VC.Admin.Validators.UserManagement;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Users)]
    public class UserManagementController : BaseApiController
    {
	    private readonly IUserService userService;
	    private readonly IOptions<AppOptions> appOptions;
		private readonly IHttpContextAccessor contextAccessor;

		public UserManagementController(IUserService userService, IOptions<AppOptions> appOptions, IHttpContextAccessor contextAccessor)
	    {
		    this.userService = userService;
		    this.appOptions = appOptions;
			this.contextAccessor = contextAccessor;
        }

	    [HttpPost]
	    public async Task<Result<PagedList<UserListItemModel>>> GetUsers([FromBody]FilterBase filter)
		{
			var result =  await userService.GetAsync(filter);
		    return new PagedList<UserListItemModel>()
		    {
				Count = result.Count,
				Items = result.Items.Select(x=> new UserListItemModel()
				{
					AgentId = x.Profile.AgentId,
					Email = x.Email,
					FullName = $"{x.FirstName} {x.LastName}",
					LastLoginDate = x.LastLoginDate,
					PublicId = x.PublicId,
					RoleIds = x.Roles.Select(y=> (RoleType)y.RoleId).ToList(),
					Status = x.Status
				}).ToList()
		    };
		}

		[HttpPost]
		public Result<ManageUserModel> CreateUserPrototype()
		{
			return new ManageUserModel();
		}

		[HttpPost]
		[ControlMode(UserManageMode.Create, typeof(UserManageSettings))]
		public async Task<Result<bool>> CreateUser([FromBody]ManageUserModel userModel)
		{
			if (!Validate(userModel))
				return false;

			var appUser = new ApplicationUser()
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Email = userModel.Email,
				Profile = new AdminProfile()
				{
					AgentId = userModel.AgentId,
					TokenExpirationDate = DateTime.Now.AddDays(appOptions.Options.ActivationTokenExpirationTermDays),
					IsConfirmed = false,
					ConfirmationToken = Guid.NewGuid()
				}
			};

			await userService.CreateAsync(appUser, userModel.RoleIds);

			return true;
		}

		[HttpPost]
		[ControlMode(UserManageMode.Update, typeof(UserManageSettings))]
		public async Task<Result<bool>> UpdateUser([FromBody]ManageUserModel userModel)
		{
			if (!Validate(userModel))
				return false;

			var user = await userService.GetAsync(userModel.PublicId);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			var context = contextAccessor.HttpContext;
			if (user.Id == Convert.ToInt32(context.User.GetUserId()) && user.Status != userModel.Status)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CurrentUserStatusUpdate]);
			}

			var isCurrentUser = user.Email.Equals(contextAccessor.HttpContext.User.GetUserName());

			user.FirstName = userModel.FirstName;
			user.LastName = userModel.LastName;
			user.Profile.AgentId = userModel.AgentId;
			user.Status = userModel.Status;
			user.Email = userModel.Email;
			user.UserName = userModel.Email;

			await userService.UpdateAsync(user, userModel.RoleIds);

			if (isCurrentUser)
			{
				await userService.RefreshSignInAsync(user);
			}

			return true;
		}

		[HttpGet]
	    public async Task<Result<ManageUserModel>> GetUser(Guid id)
		{
			var user = await userService.GetAsync(id);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}
			
			return new ManageUserModel()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				PublicId = user.PublicId,
				AgentId = user.Profile.AgentId,
				Email = user.Email,
				Status = user.Status,
				RoleIds = user.Roles.Select(y=> (RoleType)y.RoleId).ToList()
			};
		}

		[HttpPost]
	    public async Task<Result<bool>> DeleteUser([FromBody]GetUserModel getUserModel)
	    {
			var user = await userService.GetAsync(getUserModel.PublicId);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			var context = contextAccessor.HttpContext;
			if (user.Id == Convert.ToInt32(context.User.GetUserId()))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CurrentUserRemoval]);
			}

			await userService.DeleteAsync(user);

			return true;
	    }

		[HttpPost]
		public async Task<Result<bool>> ResendActivation(Guid id)
		{
			await userService.SendActivationAsync(id);

			return true;
		}

		[HttpPost]
		public async Task<Result<bool>> ResetPassword(Guid id)
		{
			await userService.SendResetPasswordAsync(id);

			return true;
		}
    }
}