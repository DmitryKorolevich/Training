using System;

namespace VitalChoice.Domain.Entities
{
	public class Comment : Entity
	{
		public DateTime CreationDate { get; set; }

		public string Text { get; set; }

		public string AuthorId { get; set; }

		public ApplicationUser Author { get; set; }
	}
}
