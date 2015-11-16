using System.Threading.Tasks;

namespace VitalChoice.Business.Mail
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, string fromDisplayName = "Vital Choice", string fromEmail = null, string toDisplayName = "",
            bool isBodyHtml = true);
    }
}