#if !NETSTANDARD1_5
using System.Net;
using System.Net.Mail;
#endif
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using System;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Mail
{
    public class EmailSender :IEmailSender, IDisposable
    {
#if !NETSTANDARD1_5
        private readonly Email _configuration;

	    private readonly SmtpClient _client;
#endif
		public EmailSender(IOptions<AppOptions> options)
	    {
#if !NETSTANDARD1_5
            _configuration = options.Value.EmailConfiguration;
		    if (!_configuration.Disabled)
		    {
		        _client = new SmtpClient(_configuration.Host, _configuration.Port)
		        {
		            UseDefaultCredentials = false,
		            DeliveryMethod = SmtpDeliveryMethod.Network,
		            EnableSsl = _configuration.Secured,
		            Credentials = new NetworkCredential(_configuration.Username, _configuration.Password)
		        };
		    }
#endif
		}

        public async Task SendEmailAsync(string email, string subject, string message, string fromDisplayName = null,
            string fromEmail = null, string toDisplayName = "",
            bool isBodyHtml = true)
        {
#if !NETSTANDARD1_5
            if (_configuration.Disabled)
                return;
            var fromEmailAdddress = fromEmail;
            if (String.IsNullOrEmpty(fromEmailAdddress))
            {
                fromEmailAdddress = _configuration.From;
            }
            if (String.IsNullOrEmpty(fromDisplayName))
            {
                fromDisplayName = "Vital Choice";
            }
            var fromAddr = new MailAddress(fromEmailAdddress, fromDisplayName, Encoding.UTF8);
            var toAddr = new MailAddress(email, toDisplayName, Encoding.UTF8);
            var mailmsg = new MailMessage(fromAddr, toAddr)
            {
                IsBodyHtml = isBodyHtml,
                Body = message,
                BodyEncoding = Encoding.UTF8,
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
            };

            try
            {
                await _client.SendMailAsync(mailmsg);
            }
            catch (SmtpFailedRecipientException)
            {

            }
#else
            await Task.Delay(0);
#endif

        }

        #region IDisposable
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
#if !NETSTANDARD1_5
                    _client?.Dispose();
#endif
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}