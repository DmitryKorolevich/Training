using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNet.Mvc;
using VitalChoice.Models.UserManagement;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Models;
using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Controllers
{
    public class UserManagementController : BaseApiController
    {
	    private static PagedModelList<UserListItemModel> _users = new PagedModelList<UserListItemModel>()
	    {
		    Items =
		    {
			    new UserListItemModel()
			    {
				    PublicId = Guid.NewGuid(),
				    Email = "gary.gould@gmail.com",
				    FirstName = "Gary",
				    LastName = "Gould",
				    RoleNames = new List<string>() {"Super Admin User"},
				    Status = "Active",
				    AgentId = "007",
					LastLoginDate = new DateTime(2015, 3, 2, 7, 5, 45)
				},
			    new UserListItemModel()
			    {
				    PublicId = Guid.NewGuid(),
				    Email = "admin@sdsd.com",
					FirstName = "Admin",
					LastName = "Admin",
				    RoleNames = new List<string>() {"Super Admin User"},
				    Status = "Active",
				    AgentId = "RW",
					LastLoginDate = new DateTime(2015, 2, 2, 2, 35, 45)
				},
			    new UserListItemModel()
			    {
				    PublicId = Guid.NewGuid(),
				    Email = "robert@gmail.com",
					FirstName = "Till",
					LastName = "Lindemann",
				    RoleNames = new List<string>() {"Content User", "Product User"},
				    Status = "Disabled",
				    AgentId = "009",
					LastLoginDate = new DateTime(2013, 1, 5, 7, 35, 45)
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "sarah@gmail.com",
					FirstName = "Sarah",
					LastName = "Cheryl",
					RoleNames = new List<string>() {"Content User"},
					Status = "Not Active",
					AgentId = "ZHB",
					LastLoginDate = null
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "jesse@gmail.com",
					FirstName = "Jesse",
					LastName = "Matney",
					RoleNames = new List<string>() {"Product User", "Order User"},
					Status = "Active",
					AgentId = "SH1",
					LastLoginDate = new DateTime(2015, 2, 2, 7, 35, 45)
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "michele@gmail.com",
					FirstName = "Michele",
					LastName = "Kinney",
					RoleNames = new List<string>() {"Content User", "Product User", "Order User", "Admin User"},
					Status = "Active",
					AgentId = "L01",
					LastLoginDate = new DateTime(2015, 2, 2, 7, 35, 45)
				},
                new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "kassandra@gmail.com",
					FirstName = "Kassandra",
					LastName = "Heal",
					RoleNames = new List<string>() {"Super Admin User"},
					Status = "Active",
					AgentId = "XLA",
					LastLoginDate = new DateTime(2015, 1, 12, 17, 35, 45)
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "heidi@gmail.com",
					FirstName = "Heidi",
					LastName = "Ward",
					RoleNames = new List<string>() {"Order User"},
					Status = "Disabled",
					AgentId = "K01",
					LastLoginDate = new DateTime(2015, 4, 2, 7, 35, 45)
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "jennifer@gmail.com",
					FirstName = "Jennifer",
					LastName = "Armstrong",
					RoleNames = new List<string>() {"Content User", "Product User"},
					Status = "Active",
					AgentId = "D01",
					LastLoginDate = new DateTime(2015, 4, 2, 7, 35, 45)
				},
                new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "robin@gmail.com",
					FirstName = "Robin",
					LastName = "Wadsworth",
					RoleNames = new List<string>() {"Admin User"},
					Status = "Active",
					AgentId = "XDK",
					LastLoginDate = new DateTime(2015, 2, 2, 7, 35, 45)
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "admin@gmail.com",
					FirstName = "Krista",
					LastName = "Coffey",
					RoleNames = new List<string>() {"Product User"},
					Status = "Active",
					AgentId = "FS1",
					LastLoginDate = new DateTime(2015, 2, 2, 7, 35, 45)
				},
				new UserListItemModel()
				{
					PublicId = Guid.NewGuid(),
					Email = "christopher@gmail.com",
					FirstName = "Christopher",
					LastName = "Reynolds",
					RoleNames = new List<string>() {"Product User", "Order User"},
					Status = "Active",
					AgentId = "XCR2",
					LastLoginDate = new DateTime(2015, 2, 2, 7, 35, 45)
				}
			}
	    };

		public UserManagementController()
	    {
	    }

		[HttpPost]
	    public Result<PagedModelList<UserListItemModel>> GetUsers([FromBody]UserManagementFilter filter)
		{
			Expression<Func<UserListItemModel, bool>> usersPredicate = x => true;

			if (!string.IsNullOrWhiteSpace(filter.Keyword))
			{
				var keyword = filter.Keyword.ToLower().Trim();

				usersPredicate = usersPredicate.And(
					x =>
						(x.FirstName + " " + x.LastName).ToLower().Contains(keyword) ||
						x.Email.ToLower().Contains(keyword));
			}

			var result = new PagedModelList<UserListItemModel>
			{
				Items = _users.Items.Where(usersPredicate.Compile()).ToList()
			};
			
			return result;
	    }

		[HttpPost]
		public Result<ManageUserModel> CreateUserPrototype()
		{
			return new ManageUserModel();
		}

		[HttpPost]
		public Result<UserListItemModel> CreateUser([FromBody]ManageUserModel userModel)
		{
			var newUser = new UserListItemModel()
			{
				PublicId = Guid.NewGuid(),
				Email = userModel.Email,
				AgentId = userModel.AgentId,
				FirstName = userModel.FirstName,
				LastName = userModel.LastName,
				Status = "Not Active",
				RoleNames = userModel.RoleNames
			};

			_users.Items.Insert(0, newUser);

			return newUser;
		}

		[HttpPost]
		public Result<UserListItemModel> UpdateUser([FromBody]ManageUserModel userModel)
		{
			var user = _users.Items.Single(x => x.PublicId == userModel.PublicId);

			user.PublicId = userModel.PublicId;
			user.Email = userModel.Email;
			user.AgentId = userModel.AgentId;
			user.FirstName = userModel.FirstName;
			user.LastName = userModel.LastName;
			user.Status = userModel.Status;
			user.RoleNames = userModel.RoleNames;

			return user;
		}

		[HttpGet]
	    public Result<ManageUserModel> GetUser(Guid id)
		{
			var user = _users.Items.Single(x => x.PublicId == id);

			return new ManageUserModel()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				PublicId = user.PublicId,
				AgentId = user.AgentId,
				Email = user.Email,
				Status = user.Status,
				RoleNames = user.RoleNames
			};
		}

		[HttpPost]
	    public Result<UserListItemModel> DeleteUser([FromBody]GetUserModel getUserModel)
	    {
		    var user = _users.Items.Single(x => x.PublicId == getUserModel.PublicId);

		    _users.Items.Remove(user);

		    return user;
	    }
    }
}