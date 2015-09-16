using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.Content
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
