namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class BugTicketEmail : EmailTemplateDataModel
    {
	    public string Customer { get; set; }
        public int Id { get; set; }
    }
}