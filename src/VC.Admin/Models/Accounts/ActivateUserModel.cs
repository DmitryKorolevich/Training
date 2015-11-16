using System;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Account
{
    public class ActivateUserModel : BaseModel
	{
	    public string AgentId { get; set; }
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
	    public string Email { get; set; }
	    public Guid PublicId { get; set; }
    }
}