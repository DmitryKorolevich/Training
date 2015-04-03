using System;
using VitalChoice.Domain;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.UserManagement;

namespace VitalChoice.Models.UserManagement
{
    public class GetUserModel //: Model<ApplicationUser,UserManageSettings>
    {
	    public Guid PublicId { get; set; }
    }
}