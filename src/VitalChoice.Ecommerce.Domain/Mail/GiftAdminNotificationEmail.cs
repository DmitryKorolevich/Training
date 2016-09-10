using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class GiftAdminNotificationEmail : EmailTemplateDataModel
    {
	    public string Recipient { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string PublicHost { get; set; }
        public IList<GiftEmailModel> Gifts { get; set; }
    }
}