using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
#if !NETSTANDARD1_5
using System.Net.Mail;
#endif

namespace VitalChoice.Business.Mail
{
#if !NETSTANDARD1_5
    public class EmailSender :IEmailSender, IDisposable
    {
        private readonly Email _configuration;
        
	    private readonly SmtpClient _client;
		public EmailSender(IOptions<AppOptions> options)
	    {
            _configuration = options.Value.EmailConfiguration;
		    if (!_configuration.Disabled)
		    {
		        _client = new SmtpClient(_configuration.Host, _configuration.Port)
		        {
		            UseDefaultCredentials = false,
		            EnableSsl = _configuration.Secured,
		            Credentials = new NetworkCredential(_configuration.Username, _configuration.Password)
		        };
		    }
		}

        public async Task SendEmailAsync(string email, string subject, string message, string fromDisplayName = null,
            string fromEmail = null, string toDisplayName = "",
            bool isBodyHtml = true)
        {
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

        }

#region IDisposable
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _client?.Dispose();
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
#else
    public class EmailSender : IEmailSender, IDisposable
    {
        public Task SendEmailAsync(string email, string subject, string message, string fromDisplayName = "Vital Choice", string fromEmail = null, string toDisplayName = "",
            bool isBodyHtml = true)
        {
            return TaskCache.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
#endif
}