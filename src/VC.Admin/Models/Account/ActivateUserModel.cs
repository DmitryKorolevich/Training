using System;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Models.Account
{
    public class ActivateUserModel : Model<ApplicationUser, IMode>
	{
	    public string AgentId { get; set; }
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
	    public string Email { get; set; }
	    public Guid PublicId { get; set; }
    }
}