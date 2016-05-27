using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace VitalChoice.Infrastructure.Domain.Entities.Roles
{
    public class ApplicationRole: IdentityRole<int>
    {
	    public UserType IdUserType { get; set; }

        public int? Order { get; set; }
    }
}
