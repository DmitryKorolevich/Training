using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Profile
{
    public class GetProfileModel : BaseModel
	{
	    public string FirstName { get; set; }

	    public string LastName { get; set; }

	    public string Email { get; set; }

	    public string AgentId { get; set; }
	}
}
