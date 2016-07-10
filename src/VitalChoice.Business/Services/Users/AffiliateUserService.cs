using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VitalChoice.Business.Mail;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Services.Users
{
	public class AffiliateUserService : UserService, IAffiliateUserService
    {
        private readonly IEcommerceRepositoryAsync<Affiliate> _affiliateRepositoryAsync;

        public AffiliateUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            VitalChoiceContext context, 
            SignInManager<ApplicationUser> signInManager, 
            ReferenceData referenceData,
            INotificationService notificationService,
            IOptions<AppOptions> options, 
            IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync,
            IEcommerceRepositoryAsync<Affiliate> affiliateRepositoryAsync,
            IUserValidator<ApplicationUser> userValidator, ITransactionAccessor<VitalChoiceContext> transactionAccessor, ILoggerProviderExtended loggerProvider) :
            base(
                userManager,
                roleManager,
                context, 
                signInManager, 
                referenceData,
                notificationService, 
                options, 
                ecommerceRepositoryAsync,
                userValidator, transactionAccessor, loggerProvider)
		{
            _affiliateRepositoryAsync = affiliateRepositoryAsync;
        }

		protected override async Task SendActivationInternalAsync(ApplicationUser dbUser)
		{
			await NotificationService.SendAffiliateActivationAsync(dbUser.Email, new UserActivation()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"https://{Options.PublicHost}/affiliateaccount/activate/{dbUser.ConfirmationToken}"
			});
		}

		protected override async Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token)
		{
			await NotificationService.SendCustomerPasswordResetAsync(dbUser.Email, new PasswordReset()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"https://{Options.PublicHost}/affiliateaccount/resetpassword/{token}"
			});
		}

        protected override async Task SendForgotPasswordInternalAsync(ApplicationUser dbUser, string token)
        {
            await NotificationService.SendUserPasswordForgotAsync(dbUser.Email, new PasswordReset() //for now email template is the same as for admin
            {
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                Link = $"https://{Options.PublicHost}/affiliateaccount/resetpassword/{token}"
            });
        }

        protected override void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles)
		{
			if (roles == null || roles.Count == 0)
			{
				return;
			}

            var ids = roles.Select(x => (int) x).Distinct().ToList();
			if (RoleManager.Roles.Any(x => x.IdUserType != UserType.Affiliate && ids.Contains(x.Id)))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToAssignWrongRole]);
			}
		}

		protected override Task ValidateUserInternalAsync(ApplicationUser user)
		{
			if (user.IdUserType!= UserType.Affiliate)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToUpdateUsingWrongService]);
			}

			return base.ValidateUserInternalAsync(user);
		}


	    protected override async Task ValidateUserOnSignIn(string login)
	    {
	        var appUser = await UserManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(login));

	        if (appUser != null)
	        {
	            var affiliate = await _affiliateRepositoryAsync.Query(p => p.Id == appUser.Id).SelectFirstOrDefaultAsync(false);
	            if (affiliate != null && affiliate.StatusCode == (int) AffiliateStatus.Pending)
	            {
	                throw new AffiliatePendingException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsNotConfirmed]);
	            }
	        }

	        await base.ValidateUserOnSignIn(login);
	    }

	    public async Task SendSuccessfulRegistration(string email, string firstName, string lastName)
		{
			await NotificationService.SendAffiliateRegistrationSuccess(email, new SuccessfulUserRegistration()
			{
				FirstName = firstName,
				LastName = lastName,
				ProfileLink = $"https://{Options.PublicHost}/affiliateprofile/index"
			});
		}

        public async Task<string> GenerateLoginTokenAsync(int id)
        {
            var user = await GetAsync(id);

            var token =
                await
                    UserManager.GenerateUserTokenAsync(user, IdentityConstants.TokenProviderName,
                        IdentityConstants.LoginFromAdminPurpose);

            return token;
        }
    }
}
