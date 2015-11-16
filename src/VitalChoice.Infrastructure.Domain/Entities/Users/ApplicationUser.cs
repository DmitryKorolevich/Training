using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VitalChoice.Infrastructure.Domain.Entities.Users
{
	public class ApplicationUser : IdentityUser<int>
	{
		public ApplicationUser()
		{
			Profile = new AdminProfile();
		}

		public Guid PublicId { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public UserStatus Status { get; set; }

		public DateTime? LastLoginDate { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime UpdatedDate { get; set; }

		public DateTime? DeletedDate { get; set; }

		public AdminProfile Profile { get; set; }

		public Guid ConfirmationToken { get; set; }

		public DateTime TokenExpirationDate { get; set; }

		public bool IsConfirmed { get; set; }

        public UserType IdUserType { get; set; }
    }
}