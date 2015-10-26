using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.Help;
using VitalChoice.Domain.Mail;

namespace VitalChoice.Business.Mail
{
    public interface INotificationService
    {
	    Task SendAdminUserActivationAsync(string email, UserActivation activation);
        
		Task SendAdminPasswordResetAsync(string email, PasswordReset passwordReset);

        Task<bool> SendBasicEmailAsync(BasicEmail email);

        Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicketEmail helpTicketEmail);

        Task SendNewBugTicketAddingForSuperAdminAsync(BugTicketEmail bugTicketEmail);

        Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicketEmail bugTicketEmail);

        Task SendCustomerActivationAsync(string email, UserActivation activation);

        Task SendCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration);

        Task SendAffiliateActivationAsync(string email, UserActivation activation);

        Task SendAffiliateRegistrationSuccess(string email, SuccessfulUserRegistration registration);

        Task SendUserPasswordForgotAsync(string email, PasswordReset passwordReset);
    }
}