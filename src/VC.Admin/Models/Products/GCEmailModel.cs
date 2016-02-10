using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Products
{
    [ApiValidator(typeof(GCManageEmailModelValidator))]
    public class GCEmailModel : BaseModel
    {
        public string ToName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string ToEmail { get; set; }

        public string FromName { get; set; }
        
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
                Subject = Subject,
                Body = Message,
                IsHTML=false,
            };

            return toReturn;
        }
    }
}