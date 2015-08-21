using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VC.Admin.Validators.Affiliate;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Mail;

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
                IsHTML=false,
            };

            return toReturn;
        }
    }
}