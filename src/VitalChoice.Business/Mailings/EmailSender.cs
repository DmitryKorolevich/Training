using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Mailings
{
#if !NETSTANDARD1_6
    public class EmailSender : IEmailSender //, IDisposable
    {
        private readonly EmailConfiguration _configuration;

        private readonly Lazy<SendGridApiClient> _client;
        private readonly ILogger _logger;

        public EmailSender(IOptions<AppOptions> options, ILoggerFactory loggerFactory)
        {
            _configuration = options.Value.EmailConfiguration;
            if (!_configuration.Disabled)
            {
                _logger = loggerFactory.CreateLogger<EmailSender>();
                _client = new Lazy<SendGridApiClient>(() => new SendGridApiClient(_configuration.ApiKey));
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, string fromDisplayName = null,
            string fromEmail = null, string toDisplayName = "",
            bool isBodyHtml = true)
        {
            if (_configuration.Disabled)
                return;
            var fromEmailAddress = !string.IsNullOrEmpty(fromEmail) ? fromEmail : _configuration.From;
            if (string.IsNullOrEmpty(fromDisplayName))
            {
                fromDisplayName = "Vital Choice";
            }

            var from = new Email(fromEmailAddress, fromDisplayName);
            var to = new Email(toEmail, toDisplayName);
            var content = new Content(isBodyHtml ? "text/html" : "text/plain", message);
            var mail = new Mail(from, subject, to, content);

            try
            {
                await _client.Value.Client.mail.send.post(requestBody: mail.GetJsonString());
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        //#region IDisposable

        //private bool _disposedValue;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!_disposedValue)
        //    {
        //        if (disposing)
        //        {
        //        }

        //        _disposedValue = true;
        //    }
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //}

        //#endregion
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