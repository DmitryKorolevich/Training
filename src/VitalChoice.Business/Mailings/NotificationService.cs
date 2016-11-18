using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Newsletters;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Mailings
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender emailSender;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEcommerceRepositoryAsync<NewsletterBlockedEmail> _newsletterBlockedEmailRepository;
        private static string _mainSuperAdminEmail;
        private static string _сustomerServiceToEmail;
        private static string _giftListEmail;
        private static string _adminHost;
        private static string _publicHost; 
        private static string _orderShippingNotificationBcc;

        public NotificationService(IEmailSender emailSender,
            IEmailTemplateService emailTemplateService,
            IEcommerceRepositoryAsync<NewsletterBlockedEmail> newsletterBlockedEmailRepository,
            IOptions<AppOptions> appOptions)
        {
            this.emailSender = emailSender;
            _emailTemplateService = emailTemplateService;
            _newsletterBlockedEmailRepository = newsletterBlockedEmailRepository;
            _mainSuperAdminEmail = appOptions.Value.MainSuperAdminEmail;
            _сustomerServiceToEmail = appOptions.Value.CustomerServiceToEmail;
            _giftListEmail = appOptions.Value.GiftListUploadEmail;
            _adminHost = appOptions.Value.AdminHost;
            _publicHost = appOptions.Value.PublicHost;
            _orderShippingNotificationBcc = appOptions.Value.OrderShippingNotificationBcc;
        }

        #region SendEmails

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
            await emailSender.SendEmailAsync(email.ToEmail, email.Subject, email.Body, email.FromName, email.FromEmail, email.ToName, email.IsHTML, email.BCCEmail);
            return true;
        }

        public async Task SendHelpTicketUpdatingEmailForCustomerServiceAsync(HelpTicketEmail helpTicketEmail)
        {
            helpTicketEmail.Url = $"https://{_publicHost}/profile/helpticket/{helpTicketEmail.Id}";

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.HelpTicketUpdateCustomerServiceNotification, helpTicketEmail);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(_сustomerServiceToEmail, generatedEmail.Subject, generatedEmail.Body);
            }
        }

        public async Task SendHelpTicketAddingEmailForCustomerAsync(string email, HelpTicketEmail helpTicketEmail)
        {
            helpTicketEmail.Url = $"https://{_publicHost}/profile/helpticket/{helpTicketEmail.Id}";

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.HelpTicketAddCustomerNotification, helpTicketEmail);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body);
            }
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
            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.UserPasswordForgot, passwordReset);

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
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body, replyToEmail: model.Email);
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

        public async Task SendEGiftNotificationEmailAsync(string email, EGiftNotificationEmail model)
        {
            if (model.EGifts != null)
            {
                for (int i = 0; i < model.EGifts.Count; i++)
                {
                    model.EGifts[i].ShowDots = i != model.EGifts.Count - 1;
                }
            }
            model.PublicHost = _publicHost;

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.EGiftNotificationEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body, toDisplayName: model.Recipient);
            }
        }

        public async Task SendGiftAdminNotificationEmailAsync(string email, GiftAdminNotificationEmail model)
        {
            if (model.Gifts != null)
            {
                for (int i = 0; i < model.Gifts.Count; i++)
                {
                    model.Gifts[i].ShowDots = i != model.Gifts.Count - 1;
                }
            }
            model.PublicHost = _publicHost;

            var generatedEmail = await _emailTemplateService.GenerateEmailAsync(EmailConstants.GiftAdminNotificationEmail, model);

            if (generatedEmail != null)
            {
                await emailSender.SendEmailAsync(email, generatedEmail.Subject, generatedEmail.Body, toDisplayName: model.Recipient);
            }
        }

        public async Task SendGiftAdminNotificationEmailsAsync(ICollection<GiftAdminNotificationEmail> models)
        {
            foreach (var model in models)
            {
                if (model.Gifts != null)
                {
                    for (int i = 0; i < model.Gifts.Count; i++)
                    {
                        model.Gifts[i].ShowDots = i != model.Gifts.Count - 1;
                    }
                }
                model.PublicHost = _publicHost;
            }

            var items = await _emailTemplateService.GenerateEmailsAsync(EmailConstants.GiftAdminNotificationEmail, models);

            if (items != null)
            {
                foreach (var item in items)
                {
                    await emailSender.SendEmailAsync(item.Key.Email, item.Value.Subject, item.Value.Body, toDisplayName: item.Key.Recipient);
                }
            }
        }

        public async Task SendGiftExpirationDateAdminNotificationEmailsAsync(ICollection<GiftAdminNotificationEmail> models)
        {
            foreach (var model in models)
            {
                if (model.Gifts != null)
                {
                    for (int i = 0; i < model.Gifts.Count; i++)
                    {
                        model.Gifts[i].ShowDots = i != model.Gifts.Count - 1;
                    }
                }
                model.PublicHost = _publicHost;
            }

            var items = await _emailTemplateService.GenerateEmailsAsync(EmailConstants.GiftExpirationDateAdminNotificationEmail, models);

            if (items != null)
            {
                foreach (var item in items)
                {
                    await emailSender.SendEmailAsync(item.Key.Email, item.Value.Subject, item.Value.Body, toDisplayName: item.Key.Recipient);
                }
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
                await emailSender.SendEmailAsync(_giftListEmail, generatedEmail.Subject, generatedEmail.Body);
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

        public async Task SendOrderShippingConfirmationEmailsAsync(ICollection<OrderShippingConfirmationEmail> models)
        {
            var items = await _emailTemplateService.GenerateEmailsAsync(EmailConstants.OrderShippingConfirmationEmail, models);

            if (items != null)
            {
                foreach (var item in items)
                {
                    await emailSender.SendEmailAsync(item.Key.ToEmail, item.Value.Subject, item.Value.Body, bccEmail: _orderShippingNotificationBcc);
                }
            }
        }

        public async Task SendOrderProductReviewEmailsAsync(ICollection<OrderProductReviewEmail> models)
        {
            var items = await _emailTemplateService.GenerateEmailsAsync(EmailConstants.OrderProductReviewEmail, models);

            if (items != null)
            {
                foreach (var item in items)
                {
                    await emailSender.SendEmailAsync(item.Key.Email, item.Value.Subject, item.Value.Body);
                }
            }
        }

        #endregion

        #region Newsletters

        public async Task<bool> IsEmailUnsubscribedAsync(int idNewsletter, string email)
        {
            return await _newsletterBlockedEmailRepository.Query(p => p.IdNewsletter == idNewsletter && p.Email == email).SelectAnyAsync();
        }

        public async Task<bool> UpdateUnsubscribeEmailAsync(int idNewsletter, string email, bool unsubscribe)
        {
            var dbItem = (await _newsletterBlockedEmailRepository.Query(p => p.IdNewsletter == idNewsletter && p.Email == email).SelectFirstOrDefaultAsync(true));
            if (unsubscribe)
            {
                if (dbItem == null)
                {
                    dbItem = new NewsletterBlockedEmail();
                    dbItem.IdNewsletter = idNewsletter;
                    dbItem.Email = email;
                    await _newsletterBlockedEmailRepository.InsertAsync(dbItem);
                }
            }
            else
            {
                if (dbItem != null)
                {
                    await _newsletterBlockedEmailRepository.DeleteAsync(dbItem);
                }
            }
            return true;
        }

        #endregion
    }
}