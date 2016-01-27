﻿using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Mail;

namespace VitalChoice.Business.Mail
{
    public interface INotificationService
    {
	    Task SendAdminUserActivationAsync(string email, UserActivation activation);
        
		Task SendAdminPasswordResetAsync(string email, PasswordReset passwordReset);

        Task SendCustomerPasswordResetAsync(string email, PasswordReset passwordReset);

        Task<bool> SendBasicEmailAsync(BasicEmail email);

        Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicketEmail helpTicketEmail);

        Task SendNewBugTicketAddingForSuperAdminAsync(BugTicketEmail bugTicketEmail);

        Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicketEmail bugTicketEmail);

        Task SendCustomerActivationAsync(string email, UserActivation activation);

        Task SendCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration);

        Task SendWholesaleCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration);

        Task SendAffiliateActivationAsync(string email, UserActivation activation);

        Task SendAffiliateRegistrationSuccess(string email, SuccessfulUserRegistration registration);

        Task SendUserPasswordForgotAsync(string email, PasswordReset passwordReset);

        Task SendCustomerServiceEmailAsync(string email, CustomerServiceEmail model);

        Task SendGCNotificationEmailAsyn(string email, GCNotificationEmail model);

        Task SendContentUrlNotificationForArticleAsync(string email, ContentUrlNotificationEmail model);

        Task SendContentUrlNotificationForRecipeAsync(string email, ContentUrlNotificationEmail model);

        Task SendPrivacyRequestEmailAsync(string email, PrivacyRequestEmail model);

        Task SendGLOrdersImportEmailAsync(GLOrdersImportEmail model);
    }
}