using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Content.ContentCrossSells
{
	public class ContentCrossSell : Entity
	{
		public ContentCrossSellType Type { get; set; }

		public string Title { get; set; }

		public decimal Price { get; set; }

		public string ImageUrl { get; set; }

		public int? IdSku { get; set; }

		public int? IdEditedBy { get; set; }

		public DateTime DateCreated { get; set; }

		public DateTime DateEdited { get; set; }

		public int Order { get; set; }
	}
}
