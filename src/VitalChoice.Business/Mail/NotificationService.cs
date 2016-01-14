using Microsoft.Extensions.OptionsModel;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Mail
{
    public class NotificationService : INotificationService
    {
	    private readonly IEmailSender emailSender;
        private static string _mainSuperAdminEmail;
        private static string _adminHost;
        private static string _publicHost;

        public NotificationService(IEmailSender emailSender, IOptions<AppOptions> appOptions)
	    {
		    this.emailSender = emailSender;
            _mainSuperAdminEmail = appOptions.Value.MainSuperAdminEmail;
            _adminHost = appOptions.Value.AdminHost;
            _publicHost = appOptions.Value.PublicHost;
        }

	    public async Task SendAdminUserActivationAsync(string email, UserActivation activation)
	    {
			//todo:refactor this to user nustache or something

			var body =
			    $"<p>Dear {activation.FirstName} {activation.LastName},</p><p>Please click the following <a href=\"{activation.Link}\">link</a> to activate your account</p><p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

		    var subject = $"Your Vital Choice User Activation";

		    await emailSender.SendEmailAsync(email, subject, body);
	    }

		public async Task SendAdminPasswordResetAsync(string email, PasswordReset passwordReset)
	    {
			//todo:refactor this to user nustache or something

			var body =
				$"<p>Dear {passwordReset.FirstName} {passwordReset.LastName},</p><p>Please click the following <a href=\"{passwordReset.Link}\">link</a> to reset your password</p><p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

			var subject = $"Your Vital Choice User Password Reset";

			await emailSender.SendEmailAsync(email, subject, body);
		}

        public async Task<bool> SendBasicEmailAsync(BasicEmail email)
        {
            //todo:refactor this to user nustache or something
            await emailSender.SendEmailAsync(email.ToEmail, email.Subject, email.Body, email.FromName, email.FromEmail, email.ToName, email.IsHTML);
            return true;
        }

        public async Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicketEmail helpTicketEmail)
        {
            //todo:refactor this to user nustache or something

            var body =
                $"<p>Dear {helpTicketEmail.Customer},</p><p>Details regarding your help desk ticket that you submitted regarding order #{helpTicketEmail.IdOrder} has been updated. <a href=\"https://{_publicHost}/profile/helpticket/{helpTicketEmail.Id}\">Please click here to review</a> or log into your Vital Choice customer profile to review your help desk tickets.</p><br/>" +
                $"<p>Please note that this is an automated message and this mailbox is not monitored. To make changes to your help desk tickets please submit a reply within the help desk ticket system found within your customer profile.</p><br/>" +
                $"<p>Sincerely,</p>" +
                $"<p>Vital Choice</p>";

            var subject = $"Your Vital Choice Help Desk #{helpTicketEmail.Id} Has Been Updated";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendNewBugTicketAddingForSuperAdminAsync(BugTicketEmail bugTicketEmail)
        {
            var body =
                $"<p>New bug ticket was added - https://{_adminHost}/help/bugs/tickets/{bugTicketEmail.Id}</p>";

            var subject = $"Vital Choice - new bug ticket was added #{bugTicketEmail.Id}";

            await emailSender.SendEmailAsync(_mainSuperAdminEmail, subject, body);
        }

        public async Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicketEmail bugTicketEmail)
        {
            var body =
                $"<p>Dear {bugTicketEmail.Customer},</p><p>Details regarding your help desk ticket that you submitted has been updated. <a href=\"https://{_adminHost}/help/bugs/tickets/{bugTicketEmail.Id}\">Please click here to review</a> or log into your Vital Choice customer profile to review your help desk tickets.</p><br/>" +
                $"<p>Please note that this is an automated message and this mailbox is not monitored. To make changes to your help desk tickets please submit a reply within the help desk ticket system found within your customer profile.</p><br/>" +
                $"<p>Sincerely,</p>" +
                $"<p>Vital Choice</p>";

            var subject = $"Your Vital Choice Bug Ticket #{bugTicketEmail.Id} Has Been Updated";

            await emailSender.SendEmailAsync(email, subject, body);
        }



        public async Task SendCustomerActivationAsync(string email, UserActivation activation)
        {
            //todo:refactor this to user nustache or something

            var body =
                $"<p>Dear {activation.FirstName} {activation.LastName},</p><p>Our records show that you recently had an account created for you. Your account is currently only available for phone orders. To begin using your account on our storefront please click the following <a href=\"{activation.Link}\">link</a> to setup a password and activate your account for online ordering</p><p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

            var subject = "Vital Choice - Setup Your Account To Order Online";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration)
	    {
			var body =
				$"<p>Dear {registration.FirstName} {registration.LastName},  thank you for registering with our store!</p>" +
				$"<p>At any time you can log into your account to check order status, update your billing information, add multiple shipping addresses, and much more. To log in, use <a href=\"{registration.ProfileLink}\">link</a></p>" +
				$"<p>Thanks again for visiting our store. Let us know if there is anything we can do to make your experience with us a better one!</p>";

			var subject = "Vital Choice - Confirmation of Customer Registration";

			await emailSender.SendEmailAsync(email, subject, body);
		}

        public async Task SendWholesaleCustomerRegistrationSuccess(string email, SuccessfulUserRegistration registration)
        {
            var body =
                $"<p>Dear {registration.FirstName} {registration.LastName},  Thank you for submitting your wholesale application. Vital Choice Seafood.</p>";

            var subject = "Vital Choice - Confirmation of Submitting Your Wholesale Application";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendAffiliateActivationAsync(string email, UserActivation activation)
        {
            //todo:refactor this to user nustache or something

            var body =
                $"<p>Dear {activation.FirstName} {activation.LastName},</p><p>Our records show that you recently had an account created for you. To begin using your account on our storefront please click the following <a href=\"{activation.Link}\">link</a> to setup a password and activate your account.</p><p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

            var subject = "Vital Choice - Setup Your Affiliate Account";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendAffiliateRegistrationSuccess(string email, SuccessfulUserRegistration registration)
        {
            var body =
                $"<p>Dear {registration.FirstName} {registration.LastName},  thank you for registering with our store!</p>" +
                $"<p>At any time you can log into your affiliate account. To log in, use <a href=\"{registration.ProfileLink}\">link</a></p>";

            var subject = "Vital Choice - Confirmation of Affiliate Registration";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendUserPasswordForgotAsync(string email, PasswordReset passwordReset)
        {
            //todo:refactor this to user nustache or something

            var body =
                $"<p>Dear {passwordReset.FirstName} {passwordReset.LastName},</p><p>Please click the following <a href=\"{passwordReset.Link}\">link</a> to setup a new password</p><p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

            var subject = "Vital Choice - Recover Your Forgot Password";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendCustomerServiceEmailAsync(string email, CustomerServiceEmail model)
        {
            var body =
                $"<p>Name - {model.Name}</p><p>Email - {model.Email}</p><p>Comment - {model.Comment}</p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

            var subject = "Vital Choice - Customer Service Email";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendGCNotificationEmailAsyn(string email, GCNotificationEmail model)
        {
            var body =
                $"<p>Your Vital Choice Gift Certificate(s):</p><p>";            
            foreach (var item in model.Data)
            {
                body += $"{item.Key}({item.Value:c} available)<br/>";
            }
            body += "</p>";

            var subject = "Your Vital Choice Gift Certificate(s)";

            await emailSender.SendEmailAsync(email, subject, body, toDisplayName: model.FirstName+" "+model.LastName);
        }

        public async Task SendContentUrlNotificationAsync(string email, ContentUrlNotificationEmail model)
        {
            var body = $"<p>Dear {model.RecipentName},</p>";
            body += $"<p>{model.FromName} ({model.FromEmail}) has recommended you read this article from Vital Choice: <a href='{model.Url}' target='_blank'>{model.Name}</a></p>";
            body += $"<p>Personal message from {model.FromName}: {model.Message}</p>";

            var subject = $"Check out this article I found on Vital Choice: {model.Name}";

            await emailSender.SendEmailAsync(email, subject, body,fromDisplayName: model.FromName, fromEmail: model.FromEmail, toDisplayName: model.RecipentName);
        }

        public async Task SendPrivacyRequestEmailAsync(string email, PrivacyRequestEmail model)
        {
            var body =
                $"<p>Name - {model.Name}</p><p>Mailing Address - {model.MailingAddress}</p><p>Other Name - {model.OtherName}</p><p>Other Address - {model.OtherAddress}</p><p>Comment - {model.Comment}</p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

            var subject = "Vital Choice - Privacy Request Email";

            await emailSender.SendEmailAsync(email, subject, body);
        }
    }
}