#if DNX451
using System.Net;
using System.Net.Mail;
#endif
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Entities.Options;
using System;

namespace VitalChoice.Business.Mail
{
    public class EmailSender :IEmailSender
    {
#if DNX451
		private readonly Email configuration;

	    private readonly SmtpClient client;
#endif
		public EmailSender(IOptions<AppOptions> options)
	    {
#if DNX451
			configuration = options.Options.EmailConfiguration;

			client = new SmtpClient(configuration.Host, configuration.Port)
			{
				UseDefaultCredentials = false,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				EnableSsl = configuration.Secured,
				Credentials = new NetworkCredential(configuration.Username, configuration.Password)
			};
#endif
		}

		public async Task SendEmailAsync(string email, string subject, string message, string fromDisplayName= "Vital Choice", string toDisplayName = "")
		{
#if DNX451
			var fromAddr = new MailAddress(configuration.From, fromDisplayName, Encoding.UTF8);
			var toAddr = new MailAddress(email, toDisplayName, Encoding.UTF8);
            var mailmsg = new MailMessage(fromAddr, toAddr)
			{
				IsBodyHtml = true,
				Body = message,
				BodyEncoding = Encoding.UTF8,
				Subject = subject,
				SubjectEncoding = Encoding.UTF8,
			};

			await client.SendMailAsync(mailmsg);
#endif
		}
	}
}