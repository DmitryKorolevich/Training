using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Business.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VC.Admin.Models.Redirects
{
    public class RedirectListItemModel : BaseModel
    {
        public int Id { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string AddedBy { get; set; }

        public string EditedBy { get; set; }

        public bool IgnoreQuery { get; set; }

        public bool FutureRedirectsExist { get; set; }

        public RedirectListItemModel(Redirect item)
        {
            if(item!=null)
            {
                Id = item.Id;
                From = item.From;
                To = item.To;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                AddedBy = item.AddedBy;
                EditedBy = item.EditedBy;
                StatusCode = item.StatusCode;
                IgnoreQuery = item.IgnoreQuery;
                FutureRedirectsExist = item.FutureRedirects.Any(p => !p.Disabled);
            }
        }
    }
}