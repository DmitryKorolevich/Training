using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Entities
{
    public class FutureRedirect
    {
        public DateTime StartDate { get; set; }

        public string Url { get; set; }
    }

    public class Redirect : Entity
    {
        public string From { get; set; }

        public string To { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public int IdAddedBy { get; set; }

        public string AddedBy { get; set; }

        public int IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool IgnoreQuery { get; set; }

        public string FutureRedirectData { get; set; }

        public ICollection<FutureRedirect> FutureRedirects
        {
            get
            {
                if (!string.IsNullOrEmpty(FutureRedirectData))
                {
                    var items = JsonConvert.DeserializeObject<ICollection<FutureRedirect>>(FutureRedirectData);
                    return items.OrderBy(p => p.StartDate).ToList();
                }
                else
                {
                    return new List<FutureRedirect>();
                }
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    FutureRedirectData = JsonConvert.SerializeObject(value);
                }
                else
                {
                    FutureRedirectData = null;
                }
            }
        }
    }
}
