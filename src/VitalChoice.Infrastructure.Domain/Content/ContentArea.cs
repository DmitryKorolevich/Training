﻿using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Content
{
    public class ContentArea : LogEntity
    {
		public string Name { get; set; }

		public DateTime Created { get; set; }

		public DateTime Updated { get; set; }

		public string Template { get; set; }

		public ApplicationUser User { get; set; }
	}
}
