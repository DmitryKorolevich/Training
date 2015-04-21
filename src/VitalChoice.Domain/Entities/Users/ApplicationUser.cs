using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VitalChoice.Domain.Entities.Users
{
	public class ApplicationUser : IdentityUser
	{
		public Guid PublicId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public UserStatus Status { get; set; }

		public DateTime? LastLoginDate { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime UpdatedDate { get; set; }

		public DateTime? DeletedDate { get; set; }

		public virtual AdminProfile Profile { get; set; }
	}
}