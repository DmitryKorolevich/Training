using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class EGiftNotificationEmail : EmailTemplateDataModel
    {
	    public string Recipient { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        [IgnoreUserTemplateUse]
        public ICollection<string> EGifts { get; set; }
        public string EGiftsBlock { get; set; }
    }
}