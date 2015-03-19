using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Domain
{
    public class ApplicationUser : IdentityUser
    {
	    public int CustomerId { get; set; }

	    public List<Comment> Comments { get; set; }
    }
}