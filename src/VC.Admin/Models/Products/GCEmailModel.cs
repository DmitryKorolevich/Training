using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(GCManageEmailModelValidator))]
    public class GCEmailModel : BaseModel
    {
        public string ToName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string ToEmail { get; set; }

        public string FromName { get; set; }

        [Localized(GeneralFieldNames.Message)]
        public string Message { get; set; }

        public BasicEmail Convert()
        {
            BasicEmail toReturn = new BasicEmail
            {
                ToName = ToName,
                ToEmail = ToEmail,
                FromName = FromName,
                Subject = "Your Vital Choice Gift Certificate(s)",
                Body = Message,
                IsHTML=false,
            };

            return toReturn;
        }
    }
}