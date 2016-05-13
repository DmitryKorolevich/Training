using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Entities.Newsletters
{
    public class Newsletter : Entity
	{
        public string Name { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public ICollection<NewsletterBlockedEmail> BlockedEmails { get; set; }
    }
}
