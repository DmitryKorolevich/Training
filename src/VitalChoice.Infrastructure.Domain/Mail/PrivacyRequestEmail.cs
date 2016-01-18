using VitalChoice.Ecommerce.Domain.Mail;

namespace VitalChoice.Infrastructure.Domain.Mail
{
    public class PrivacyRequestEmail : EmailTemplateDataModel
    {
	    public string Name { get; set; }
        public string MailingAddress { get; set; }
        public string OtherName { get; set; }
        public string OtherAddress { get; set; }
        public string Comment { get; set; }
    }
}