using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Models.UserManagement;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Models;
using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Data.Extensions;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Controllers
{
    public class UserManagementController : BaseApiController
    {
	    private readonly IUserService userService;

	    public UserManagementController(IUserService userService)
	    {
		    this.userService = userService;
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
					RoleIds = x.Roles.Select(y=> (RoleType)Enum.Parse(typeof(RoleType), y.RoleId)).ToList()
				})
		    };
		}

		[HttpPost]
		public Result<ManageUserModel> CreateUserPrototype()
		{
			return new ManageUserModel();
		}

		[HttpPost]
		public async Task<Result<bool>> CreateUser([FromBody]ManageUserModel userModel)
		{
			var appUser = new ApplicationUser()
			{
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Email = userModel.Email,
				Profile = new AdminProfile()
				{
					AgentId = userModel.AgentId
				}
			};

			await userService.CreateAsync(appUser, userModel.RoleIds);

			return true;
		}

		[HttpPost]
		public async Task<Result<bool>> UpdateUser([FromBody]ManageUserModel userModel)
		{
			var user = await userService.GetAsync(userModel.PublicId);
			if (user == null)
			{
				throw new ApiException();
			}

			user.FirstName = userModel.FirstName;
			user.LastName = userModel.LastName;
			user.Profile.AgentId = userModel.AgentId;
			user.Status = userModel.Status;
			user.Email = userModel.Email;

			await userService.UpdateAsync(user, userModel.RoleIds);

			return true;
		}

		[HttpGet]
	    public async Task<Result<ManageUserModel>> GetUser(Guid id)
		{
			var user = await userService.GetAsync(id);
			if (user == null)
			{
				throw new ApiException();
			}
			
			return new ManageUserModel()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				PublicId = user.PublicId,
				AgentId = user.Profile.AgentId,
				Email = user.Email,
				Status = user.Status,
				RoleIds = user.Roles.Select(y => (RoleType)Enum.Parse(typeof(RoleType), y.RoleId)).ToList()
			};
		}

		[HttpPost]
	    public async Task<Result<bool>> DeleteUser([FromBody]GetUserModel getUserModel)
	    {
			var user = await userService.GetAsync(getUserModel.PublicId);
			if (user == null)
			{
				throw new ApiException();
			}

			await userService.DeleteAsync(user);

			return true;
	    }
    }
}