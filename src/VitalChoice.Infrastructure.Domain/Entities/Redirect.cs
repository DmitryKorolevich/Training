using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Entities
{
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
    }
}
