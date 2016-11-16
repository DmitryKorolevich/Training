using System;
using System.Collections.Generic;
using VC.Admin.Validators.Product;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Infrastructure.Domain.Entities;
using VC.Admin.Validators.Redirect;

namespace VC.Admin.Models.Redirects
{
    [ApiValidator(typeof(RedirectManageModelValidator))]
    public class RedirectManageModel : BaseModel
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.Url)]
        public string From { get; set; }

        [Localized(GeneralFieldNames.RedirectUrl)]
        public string To { get; set; }

        public bool IgnoreQuery { get; set; }

        public ICollection<FutureRedirect> FutureRedirects { get; set; }

        public RedirectManageModel()
        {
        }

        public RedirectManageModel(Redirect item)
        {
            if (item != null)
            {
                Id = item.Id;
                From = item.From;
                To = item.To;
                IgnoreQuery = item.IgnoreQuery;
                FutureRedirects = item.FutureRedirects;
            }
        }

        public Redirect Convert()
        {
            Redirect toReturn = new Redirect();
            toReturn.Id = Id;
            toReturn.From = From;
            if(!toReturn.From.StartsWith("/"))
            {
                toReturn.From = "/" + toReturn.From;
            }
            toReturn.To = To;
            toReturn.IgnoreQuery = IgnoreQuery;
            toReturn.FutureRedirects = FutureRedirects;

            return toReturn;
        }
    }
}