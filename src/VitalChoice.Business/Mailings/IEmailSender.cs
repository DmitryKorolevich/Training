using System.Threading.Tasks;

namespace VitalChoice.Business.Mailings
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message, string fromDisplayName = "Vital Choice", string fromEmail = null, string toDisplayName = "",
            bool isBodyHtml = true);
    }
}