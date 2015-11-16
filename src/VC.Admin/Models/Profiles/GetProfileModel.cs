using VitalChoice.Validation.Models;

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
