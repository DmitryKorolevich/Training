using System;

namespace VitalChoice.Domain.Entities.Users
{
    public class AdminProfile : Entity
    {
		public string AgentId { get; set; }

	    public Guid ConfirmationToken { get; set; }

		public DateTime TokenExpirationDate { get; set; }

	    public bool IsConfirmed { get; set; }

	    public ApplicationUser User { get; set; }
	}
}