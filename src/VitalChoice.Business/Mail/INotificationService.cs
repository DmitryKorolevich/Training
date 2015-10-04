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

        Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicket helpTicket);

        Task SendNewBugTicketAddingForSuperAdminAsync(BugTicket bugTicket);

        Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicket bugTicket);
    }
}