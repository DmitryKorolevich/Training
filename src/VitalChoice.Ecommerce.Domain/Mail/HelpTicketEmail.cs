namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class HelpTicketEmail
    {
	    public string Customer { get; set; }
        public int Id { get; set; }
        public int IdOrder { get; set; }
    }
}