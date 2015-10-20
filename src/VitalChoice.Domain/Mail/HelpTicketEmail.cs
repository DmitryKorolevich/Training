using System;

namespace VitalChoice.Domain.Mail
{
    public class HelpTicketEmail
    {
	    public string Customer { get; set; }
        public int Id { get; set; }
        public int IdOrder { get; set; }
    }
}