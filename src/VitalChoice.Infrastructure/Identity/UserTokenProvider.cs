using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Infrastructure.Identity
{
    public class UserTokenProvider: IUserTokenProvider<ApplicationUser>
    {
	    private readonly IOptions<AppOptions> appOptions;

	    public UserTokenProvider(IOptions<AppOptions> appOptions)
	    {
		    this.appOptions = appOptions;
	    }

	    public async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
		    user.TokenExpirationDate = DateTime.Now.AddDays(appOptions.Value.ActivationTokenExpirationTermDays);
			if (purpose != IdentityConstants.ForgotPasswordResetPurpose && purpose != IdentityConstants.CustomerLoginPurpose)
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

		    if (purpose != IdentityConstants.ForgotPasswordResetPurpose && purpose != IdentityConstants.PasswordResetPurpose && purpose != IdentityConstants.CustomerLoginPurpose)
		    {
				valid = valid && !user.IsConfirmed;
		    }

			return Task.FromResult(valid);
		}

	    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
			return Task.FromResult(false);
		}

	    public string Name => "Default Token Provider";
    }
}
