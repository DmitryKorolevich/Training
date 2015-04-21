using System;

namespace VitalChoice.Domain.Entities.Users
{
    public class AdminProfile
    {
		public string Id { get; set; }

		public string AgentId { get; set; }

		public virtual ApplicationUser User { get; set; }
	}
}