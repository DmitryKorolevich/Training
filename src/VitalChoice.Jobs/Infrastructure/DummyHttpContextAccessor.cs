using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;

namespace VitalChoice.Jobs.Infrastructure
{
    public class DummyHttpContextAccessor:IHttpContextAccessor
    {
	    public HttpContext HttpContext { get; set; }

	    public DummyHttpContextAccessor()
	    {
			HttpContext = new DefaultHttpContext();
	    }
    }
}
