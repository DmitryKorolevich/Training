using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using VC.Admin.Models.UserManagement;
using VC.Admin.Validators.UserManagement;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Attributes;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VC.Admin.Controllers
{
    public class UserManagementController : BaseApiController
    {
	    private readonly IAdminUserService userService;
	    private readonly IOptions<AppOptions> appOptions;

		public UserManagementController(IAdminUserService userService, IOptions<AppOptions> appOptions)
	    {
		    this.userService = userService;
		    this.appOptions = appOptions;
        }

	    [HttpGet]
	    public async Task<Result<ICollection<AdminTeam>>> GetAdminTeams()
	    {
	        return (await userService.GetAdminTeams()).ToList();
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
                    Id=x.Id,
					AgentId = x.Profile.AgentId,
					Email = x.Email,
					FullName = $"{x.FirstName} {x.LastName}",
					LastLoginDate = x.LastLoginDate,
					PublicId = x.PublicId,
					RoleIds = x.Roles.Select(y=> (RoleType)y.RoleId).ToList(),
					Status = x.Status,
                    IdAdminTeam = x?.Profile?.AdminTeam?.Id,
                    AdminTeam = x?.Profile?.AdminTeam?.Name
				}).ToList()
		    };
		}

        [AdminAuthorize(PermissionType.Users)]
        [HttpPost]
		public Result<ManageUserModel> CreateUserPrototype([FromBody] object model)
		{
			return new ManageUserModel();
		}

        [AdminAuthorize(PermissionType.Users)]
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
				TokenExpirationDate = DateTime.Now.AddDays(appOptions.Value.ActivationTokenExpirationTermDays),
				IsConfirmed = false,
				ConfirmationToken = Guid.NewGuid(),
                Profile = new AdminProfile()
				{
					AgentId = userModel.AgentId,
                    IdAdminTeam = userModel.IdAdminTeam
				},
				IdUserType = UserType.Admin,
				Status = UserStatus.NotActive,
			};

			await userService.CreateAsync(appUser, userModel.RoleIds);

			return true;
		}

        [AdminAuthorize(PermissionType.Users)]
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

			var context = HttpContext;
			if (user.Id == Convert.ToInt32(context.User.GetUserId()) && user.Status != userModel.Status)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CurrentUserStatusUpdate]);
			}

			var isCurrentUser = user.Email.Equals(HttpContext.User.GetUserName());

			user.FirstName = userModel.FirstName;
			user.LastName = userModel.LastName;
			user.Profile.AgentId = userModel.AgentId;
            user.Profile.IdAdminTeam = userModel.IdAdminTeam;
            user.Status = userModel.Status;
			user.Email = userModel.Email;
			user.UserName = userModel.Email;
			user.IdUserType = UserType.Admin;

			await userService.UpdateAsync(user, userModel.RoleIds);

			if (isCurrentUser)
			{
				await userService.RefreshSignInAsync(user);
			}

			return true;
		}

        [AdminAuthorize(PermissionType.Users)]
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
                IdAdminTeam = user?.Profile?.IdAdminTeam,
                Email = user.Email,
				Status = user.Status,
				RoleIds = user.Roles.Select(y=> (RoleType)y.RoleId).ToList(),
			};
		}

        [AdminAuthorize(PermissionType.Users)]
        [HttpPost]
	    public async Task<Result<bool>> DeleteUser([FromBody]GetUserModel getUserModel)
	    {
			var user = await userService.GetAsync(getUserModel.PublicId);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			var context = HttpContext;
			if (user.Id == Convert.ToInt32(context.User.GetUserId()))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CurrentUserRemoval]);
			}

			await userService.DeleteAsync(user);

			return true;
	    }

        [AdminAuthorize(PermissionType.Users)]
        [HttpPost]
		public async Task<Result<bool>> ResendActivation(Guid id, [FromBody] object model)
		{
			await userService.ResendActivationAsync(id);

			return true;
		}

        [AdminAuthorize(PermissionType.Users)]
        [HttpPost]
		public async Task<Result<bool>> ResetPassword(Guid id, [FromBody] object model)
		{
			await userService.SendResetPasswordAsync(id);

			return true;
		}
    }
}