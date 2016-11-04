using System.Threading.Tasks;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.Profile;
using VC.Admin.Validators.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Controllers
{
	[AdminAuthorize]
    public class ProfileController : BaseApiController
    {
	    private readonly IAdminUserService _userService;
	    private readonly ExtendedUserManager _userManager;

	    public ProfileController(IAdminUserService userService, ExtendedUserManager userManager)
	    {
	        this._userService = userService;
	        _userManager = userManager;
	    }

	    [HttpPost]
		[ControlMode(UpdateProfileMode.Default, typeof(UpdateProfileSettings))]
		public async Task<Result<GetProfileModel>> UpdateProfile([FromBody]UpdateProfileModel profileModel)
		{
		    if (User.Identity.IsAuthenticated)
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

			    var oldEmail = user.Email;

				user.FirstName = profileModel.FirstName;
				user.LastName = profileModel.LastName;
				user.Email = profileModel.Email;
			    user.UserName = profileModel.Email;

			    user = profileModel.Mode.Mode == UpdateProfileMode.WithPassword ? await _userService.UpdateWithPasswordChangeAsync(user, profileModel.OldPassword, profileModel.NewPassword)
				    : await _userService.UpdateAsync(user);

			    if (oldEmail != user.Email)
			    {
				    await _userService.RefreshSignInAsync(user);
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