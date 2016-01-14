namespace VitalChoice.Infrastructure.Domain.Mail
{
    public class PrivacyRequestEmail
    {
	    public string Name { get; set; }
        public string MailingAddress { get; set; }
        public string OtherName { get; set; }
        public string OtherAddress { get; set; }
        public string Comment { get; set; }
    }
}