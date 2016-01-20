using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Mail
{
    public class GCNotificationEmail : EmailTemplateDataModel
    {
	    public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [IgnoreUserTemplateUse]
        public IDictionary<string,decimal> Data { get; set; }
        public string BalancesBlock { get; set; }
    }
}