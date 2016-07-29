using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VC.Admin.Validators.Affiliate;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;

namespace VC.Admin.Models.Affiliate
{
    [ApiValidator(typeof(AffiliateEmailManageModelValidator))]
    public class AffiliateEmailModel : BaseModel
    {
        public string ToName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string ToEmail { get; set; }

        public string FromName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string FromEmail { get; set; }

        public string Subject { get; set; }

        [Localized(GeneralFieldNames.Message)]
        public string Message { get; set; }

        public BasicEmail Convert()
        {
            BasicEmail toReturn = new BasicEmail
            {
                ToName = ToName,
                ToEmail = ToEmail,
                FromName = FromName,
                FromEmail = FromEmail,
                Subject = Subject,
                Body = Message,
                IsHTML=true,
            };

            return toReturn;
        }
    }
}