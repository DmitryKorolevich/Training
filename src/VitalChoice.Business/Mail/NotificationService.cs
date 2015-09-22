using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.Help;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Mail;

namespace VitalChoice.Business.Mail
{
    public class NotificationService : INotificationService
    {
	    private readonly IEmailSender emailSender;
        private static string _mainSuperAdminEmail;
        private static string _adminHost;

        public NotificationService(IEmailSender emailSender, IOptions<AppOptions> appOptions)
	    {
		    this.emailSender = emailSender;
            _mainSuperAdminEmail = appOptions.Options.MainSuperAdminEmail;
            _adminHost = appOptions.Options.AdminHost;
        }

	    public async Task SendUserActivationAsync(string email, UserActivation activation)
	    {
			//todo:refactor this to user nustache or something

			var body =
			    $"<p>Dear {activation.FirstName} {activation.LastName},</p><p>Please click the following <a href=\"{activation.Link}\">link</a> to activate your account</p><p></p><p>Vital Choice Administration,</p><p></p><p>This is an automated message. Do not reply. This mailbox is not monitored.</p>";

		    var subject = $"Your Vital Choice User Activation";

		    await emailSender.SendEmailAsync(email, subject, body);
	    }

	    public async Task SendPasswordResetAsync(string email, PasswordReset passwordReset)
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

        public async Task SendHelpTicketUpdatingEmailForCustomerAsync(string email, HelpTicket helpTicket)
        {
            //todo:refactor this to user nustache or something

            var body =
                $"<p>Dear {helpTicket.Customer},</p><p>Details regarding your help desk ticket that you submitted regarding order #{helpTicket.IdOrder} has been updated. Please click here to review or log into your Vital Choice customer profile to review your help desk tickets.</p><br/>" +
                $"<p>Please note that this is an automated message and this mailbox is not monitored. To make changes to your help desk tickets please submit a reply within the help desk ticket system found within your customer profile.</p><br/>" +
                $"<p>Sincerely,</p>" +
                $"<p>Vital Choice</p>";

            var subject = $"Your Vital Choice Help Desk #{helpTicket.Id} Has Been Updated";

            await emailSender.SendEmailAsync(email, subject, body);
        }

        public async Task SendNewBugTicketAddingForSuperAdminAsync(BugTicket bugTicket)
        {
            var body =
                $"<p>New bug ticket was added - {_adminHost}help/bugs/{bugTicket.Id}</p>";

            var subject = $"Vital Choice - new bug ticket was added #{bugTicket.Id}";

            await emailSender.SendEmailAsync(_mainSuperAdminEmail, subject, body);
        }

        public async Task SendBugTicketUpdatingEmailForAuthorAsync(string email, BugTicket bugTicket)
        {
            var body =
                $"<p>Dear {bugTicket.AddedBy},</p><p>Details regarding your help desk ticket that you submitted has been updated. Please click here to review or log into your Vital Choice customer profile to review your help desk tickets.</p><br/>" +
                $"<p>Please note that this is an automated message and this mailbox is not monitored. To make changes to your help desk tickets please submit a reply within the help desk ticket system found within your customer profile.</p><br/>" +
                $"<p>Sincerely,</p>" +
                $"<p>Vital Choice</p>";

            var subject = $"Your Vital Choice Help Desk #{bugTicket.Id} Has Been Updated";

            await emailSender.SendEmailAsync(email, subject, body);
        }
    }
}