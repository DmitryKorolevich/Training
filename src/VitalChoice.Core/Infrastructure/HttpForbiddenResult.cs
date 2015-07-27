using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.WebUtilities;

namespace VitalChoice.Core.Infrastructure
{
	public class HttpForbiddenResult : HttpStatusCodeResult
	{
		public HttpForbiddenResult()
			: base(StatusCodes.Status403Forbidden)
		{
		}
	}
}
