using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Users
{
    public class AdminProfile : Entity
    {
		public string AgentId { get; set; }

	    public ApplicationUser User { get; set; }

        public int? IdAdminTeam { get; set; }

        public AdminTeam AdminTeam { get; set; }
    }
}