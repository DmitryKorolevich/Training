using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class GiftEmailModel
    {
        public string Code { get; set; }
        public decimal Amount { get; set; }
        public bool ShowDots { get; set; }
    }

    public class EGiftNotificationEmail : EmailTemplateDataModel
    {
        public string Sender { get; set; }
	    public string Recipient { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string PublicHost { get; set; }
        public IList<GiftEmailModel> EGifts { get; set; }
    }
}