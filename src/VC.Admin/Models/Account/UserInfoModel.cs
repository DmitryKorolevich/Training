using System;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Models.Account
{
    public class UserInfoModel : Model<ApplicationUser, IMode>
	{
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
    }
}