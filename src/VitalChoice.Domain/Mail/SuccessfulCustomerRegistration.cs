using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Mail
{
    public class SuccessfulCustomerRegistration
    {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ProfileLink { get; set; }
	}
}
