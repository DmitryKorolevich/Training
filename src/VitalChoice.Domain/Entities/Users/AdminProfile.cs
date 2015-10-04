using System;

namespace VitalChoice.Domain.Entities.Users
{
    public class AdminProfile : Entity
    {
		public string AgentId { get; set; }

	    public ApplicationUser User { get; set; }
	}
}