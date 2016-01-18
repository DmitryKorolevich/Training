namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class ContentUrlNotificationEmail : EmailTemplateDataModel
    {
        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string RecipentName { get; set; }

        public string Message { get; set; }
        
        public string Url { get; set; }
        
        public string Name { get; set; }
    }
}