using Microsoft.AspNet.Identity;

namespace VitalChoice.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
	    public int CustomerId { get; set; }
    }
}