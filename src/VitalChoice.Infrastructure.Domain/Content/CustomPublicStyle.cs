using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Content
{
    public class CustomPublicStyle : Entity
	{
		public string Name { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

		public string Styles { get; set; }

		public int? IdEditedBy { get; set; }

		public ApplicationUser User { get; set; }
	}
}
