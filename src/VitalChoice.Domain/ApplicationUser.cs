using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Domain
{
    public class ApplicationUser : IdentityUser//, IObjectState
    {
	    public int CustomerId { get; set; }

	    public List<Comment> Comments { get; set; }
    }
}