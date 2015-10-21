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

		Task SendStorefrontUserActivationAsync(string email, UserActivation activation);

		Task SendAdminPasswordResetAsync(string email, PasswordReset passwordReset);

        Task<bool> SendBasicEmailAsync(BasicEmail email);

        Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicketEmail helpTicketEmail);

        Task SendNewBugTicketAddingForSuperAdminAsync(BugTicketEmail bugTicketEmail);

        Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicketEmail bugTicketEmail);

	    Task SendCustomerRegistrationSuccess(string email, SuccessfulCustomerRegistration registration);
    }
}