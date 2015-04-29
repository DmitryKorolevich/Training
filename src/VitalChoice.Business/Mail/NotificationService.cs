using System;
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
			    $"<p>Dear {activation.FirstName} {activation.LastName},</p><p>Please use <a href=\"{activation.Link}\">link</a> to activate your account</p><p></p><p>Best Regards,</p><p>Vital Choice Administration</p>";

		    var subject = $"Vital Choice admin account activation";

		    await emailSender.SendEmailAsync(email, subject, body);
	    }
    }
}