using System;
using VitalChoice.Domain;
using VitalChoice.Validation.Models;

namespace VitalChoice.Models.UserManagement
{
    public class GetUserModel //: Model<ApplicationUser,UserManageSettings>
    {
	    public Guid PublicId { get; set; }
    }
}