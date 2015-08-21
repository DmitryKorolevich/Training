using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Mail;

namespace VitalChoice.Business.Mail
{
    public class NotificationService : INotificationService
    {
	    private readonly IEmailSender emailSender;

	    public NotificationService(IEmailSender emailSender)
	    {
		    this.emailSender = emailSender;
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

        public async Task SendBasicEmailAsync(BasicEmail email)
        {
            //todo:refactor this to user nustache or something
            await emailSender.SendEmailAsync(email.ToEmail, email.Subject, email.Body, email.FromName, email.FromEmail, email.ToName, email.IsHTML);
        }
    }
}