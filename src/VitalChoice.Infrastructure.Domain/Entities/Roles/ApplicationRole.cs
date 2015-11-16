using Microsoft.AspNet.Identity.EntityFramework;

namespace VitalChoice.Infrastructure.Domain.Entities.Roles
{
    public class ApplicationRole: IdentityRole<int>
    {
	    public UserType IdUserType { get; set; }
    }
}
