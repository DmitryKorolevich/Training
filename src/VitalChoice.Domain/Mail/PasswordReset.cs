using System;

namespace VitalChoice.Domain.Mail
{
    public class PasswordReset
    {
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
	    public string Link { get; set; }
    }
}