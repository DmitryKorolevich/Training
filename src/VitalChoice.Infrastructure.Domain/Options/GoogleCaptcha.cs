using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class GoogleCaptcha
    {
	    public string PublicKey { get; set; }

	    public string SecretKey { get; set; }

	    public string VerifyUrl { get; set; }
    }
}
