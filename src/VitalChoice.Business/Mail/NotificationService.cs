﻿using Microsoft.Extensions.OptionsModel;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Mail
{
    public class NotificationService : INotificationService
    {
	    private readonly IEmailSender emailSender;
        private readonly IEmailTemplateService _emailTemplateService;
        private static string _mainSuperAdminEmail;
        private static string _adminHost;
        private static string _publicHost;

        public NotificationService(IEmailSender emailSender,
            IEmailTemplateService emailTemplateService,
            IOptions<AppOptions> appOptions)
	    {
		    this.emailSender = emailSender;
            _emailTemplateService = emailTemplateService;
            _mainSuperAdminEmail = appOptions.Value.MainSuperAdminEmail;
            _adminHost = appOptions.Value.AdminHost;
            _publicHost = appOptions.Value.PublicHost;
        }

	    public async Task SendAdminUserActivationAsync(string email, UserActivation activation)
	    {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.AdminRegistration, activation);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
	    }

		public async Task SendAdminPasswordResetAsync(string email, PasswordReset passwordReset)
	    {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.AdminPasswordForgot, passwordReset);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendCustomerPasswordResetAsync(string email, PasswordReset passwordReset)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.CustomerPasswordResetViaAdmin, passwordReset);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task<bool> SendBasicEmailAsync(BasicEmail email)
        {
            await emailSender.SendEmailAsync(email.ToEmail, email.Subject, email.Body, email.FromName, email.FromEmail, email.ToName, email.IsHTML);
            return true;
        }

        public async Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicketEmail helpTicketEmail)
        {
            helpTicketEmail.Url = $"https://{_publicHost}/profile/helpticket/{helpTicketEmail.Id}";

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.HelpTicketUpdateCustomerNotification, helpTicketEmail);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendBugTicketUpdaingForSuperAdminAsync(BugTicketEmail bugTicketEmail)
        {
            bugTicketEmail.Url = $"https://{_adminHost}/help/bugs/tickets/{bugTicketEmail.Id}";

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.BugTicketUpdateSuperAdminNotification, bugTicketEmail);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(_mainSuperAdminEmail, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicketEmail bugTicketEmail)
        {
            bugTicketEmail.Url = $"https://{_adminHost}/help/bugs/tickets/{bugTicketEmail.Id}";

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.BugTicketUpdateAuthorNotification, bugTicketEmail);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendCustomerActivationAsync(string email, UserActivation activation)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.CustomerRegistrationViaAdmin, activation);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration)
	    {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.CustomerRegistrationViaWeb, registration);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
		}

        public async Task SendWholesaleCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.CustomerWholesaleRegistrationViaWeb, registration);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendAffiliateActivationAsync(string email, UserActivation activation)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.AffiliateRegistrationViaAdmin, activation);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendAffiliateRegistrationSuccess(string email, SuccessfulUserRegistration registration)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.AffiliateRegistrationViaWeb, registration);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendUserPasswordForgotAsync(string email, PasswordReset passwordReset)
        {
            var generatedEmail =  await _emailTemplateService.GenerateEmailAsync(EmailConstants.UserPasswordForgot, passwordReset);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendCustomerServiceEmailAsync(string email, CustomerServiceEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.CustomerServiceRequestEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendGCNotificationEmailAsyn(string email, GCNotificationEmail model)
        {
            model.BalancesBlock = "<p>";
            foreach (var item in model.Data)
            {
                model.BalancesBlock += $"{item.Key}({item.Value:c} available)<br/>";
            }
            model.BalancesBlock += "</p>";

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.HealthwiseSendGCEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body, toDisplayName: model.FirstName + " " + model.LastName);
            }
        }

        public async Task SendContentUrlNotificationForArticleAsync(string email, ContentUrlNotificationEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.EmailArticle, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body, fromDisplayName: model.FromName, fromEmail: model.FromEmail, toDisplayName: model.RecipentName);
            }
        }

        public async Task SendContentUrlNotificationForRecipeAsync(string email, ContentUrlNotificationEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.EmailRecipe, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body, fromDisplayName: model.FromName, fromEmail: model.FromEmail, toDisplayName: model.RecipentName);
            }
        }

        public async Task SendPrivacyRequestEmailAsync(string email, PrivacyRequestEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.PrivacyRequestEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendGLOrdersImportEmailAsync(GLOrdersImportEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.GLOrdersImportEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(_mainSuperAdminEmail, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendOrderConfirmationEmailAsync(string email, OrderConfirmationEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.OrderConfirmationEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendOrderShippingConfirmationEmailAsync(string email, OrderShippingConfirmationEmail model)
        {
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.OrderShippingConfirmationEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
        }
    }
}