using System;

namespace VitalChoice.Domain.Mail
{
    public struct UserActivation
    {
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
	    public string Link { get; set; }
    }
}