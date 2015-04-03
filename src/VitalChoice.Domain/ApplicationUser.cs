using System.Collections.Generic;
using VitalChoice.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace VitalChoice.Domain
{
    public class ApplicationUser : IdentityUser
    {
	    public int CustomerId { get; set; }
    }
}