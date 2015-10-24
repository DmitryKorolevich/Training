using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VitalChoice.Domain.Entities.Roles
{
    public class ApplicationRole: IdentityRole<int>
    {
	    public UserType IdUserType { get; set; }
    }
}
