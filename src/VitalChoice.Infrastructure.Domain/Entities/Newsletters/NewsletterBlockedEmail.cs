using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Entities.Newsletters
{
    public class NewsletterBlockedEmail : Entity
	{
        public int IdNewsletter { get; set; }

        public string Email { get; set; }
    }
}
