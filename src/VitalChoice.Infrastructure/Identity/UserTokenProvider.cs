using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Infrastructure.Identity
{
    public class UserTokenProvider: IUserTwoFactorTokenProvider<ApplicationUser>
    {
	    private readonly IOptions<AppOptions> _appOptions;

	    public UserTokenProvider(IOptions<AppOptions> appOptions)
	    {
		    _appOptions = appOptions;
	    }

	    public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
		    user.TokenExpirationDate = DateTime.Now.AddDays(_appOptions.Value.ActivationTokenExpirationTermDays);
			if (purpose != IdentityConstants.ForgotPasswordResetPurpose && purpose != IdentityConstants.LoginFromAdminPurpose)
			{
				user.IsConfirmed = false;
			}
			user.ConfirmationToken = Guid.NewGuid();

		    var result = await manager.UpdateAsync(user);
		    if (!result.Succeeded)
		    {
			    throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.GenerateSecurityStampError]);
		    }

			return user.ConfirmationToken.ToString();
		}

	    public Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
		    var valid = user.ConfirmationToken.Equals(Guid.Parse(token)) &&
		                user.TokenExpirationDate.Subtract(DateTime.Now).Days > 0;

		    if (purpose != IdentityConstants.ForgotPasswordResetPurpose && purpose != IdentityConstants.PasswordResetPurpose && purpose != IdentityConstants.LoginFromAdminPurpose)
		    {
				valid = valid && !user.IsConfirmed;
		    }

			return Task.FromResult(valid);
		}

	    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
			return TaskCache<bool>.DefaultCompletedTask;
		}

	    public string Name => "Default Token Provider";
    }
}
