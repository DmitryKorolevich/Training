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

namespace VC.Admin.Models.Product
{
    [ApiValidator(typeof(GCManageEmailModelValidator))]
    public class GCEmailModel : Model<GiftCertificateEmail, IMode>
    {
        public string ToName { get; set; }

        [Localized(GeneralFieldNames.Email)]
        public string ToEmail { get; set; }

        public string FromName { get; set; }

        [Localized(GeneralFieldNames.Message)]
        public string Message { get; set; }

        public GCEmailModel()
        {
        }

        public override GiftCertificateEmail Convert()
        {
            GiftCertificateEmail toReturn = new GiftCertificateEmail();
            toReturn.ToName = ToName;
            toReturn.ToEmail = ToEmail;
            toReturn.FromName = FromName;
            toReturn.Message = Message;

            return toReturn;
        }
    }
}