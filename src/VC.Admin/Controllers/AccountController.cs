using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using VitalChoice.Admin.Models;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Models.Account;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Models;

namespace VitalChoice.Controllers
{
    public class AccountController : BaseApiController
    {
	    private readonly IUserService userService;

	    public AccountController(IUserService userService)
	    {
		    this.userService = userService;
	    }

		[HttpGet]
	    public async Task<Result<ActivateUserModel>> GetUser(Guid id)
		{
			var result = await userService.GetByTokenAsync(id);
			if (result == null)
			{
				throw new ApiException();
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
		    var user = await userService.GetAsync(model.PublicId);
		    if (user == null)
		    {
			    throw new ApiException();
		    }

		    user.FirstName = model.FirstName;
		    user.LastName = model.LastName;
		    user.Email = model.Email;
		    user.Profile.IsConfirmed = true;
			user.Status = UserStatus.Active;

		    await userService.UpdateAsync(user, null, model.Password);

		    await userService.SignInAsync(user);

		    return new UserInfoModel()
		    {
			    FirstName = user.FirstName,
				LastName = user.LastName
		    };
	    }
	}
}