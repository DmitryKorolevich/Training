namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class HelpTicketEmail : EmailTemplateDataModel
    {
	    public string Customer { get; set; }
        public int Id { get; set; }
        public int IdOrder { get; set; }
        public string Url { get; set; }
    }
}