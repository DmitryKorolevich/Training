using System.Collections.Generic;
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

        [Localized(GeneralFieldNames.Message)]
        public string Message { get; set; }

        public IList<GiftEmailModel> Gifts { get; set; }

        public GiftAdminNotificationEmail Convert()
        {
            GiftAdminNotificationEmail toReturn = new GiftAdminNotificationEmail
            {
                Recipient = ToName,
                Email = ToEmail,
                Message = Message,
                Gifts = Gifts
            };

            return toReturn;
        }
    }
}