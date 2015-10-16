using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;

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
		    user.IsConfirmed = false;
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
		    return Task.FromResult(user.ConfirmationToken.Equals(Guid.Parse(token)) && !user.IsConfirmed && user.TokenExpirationDate.Subtract(DateTime.Now).Days > 0);
		}

	    public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
			return Task.FromResult(false);
		}

	    public string Name => "Default Token Provider";
    }
}
