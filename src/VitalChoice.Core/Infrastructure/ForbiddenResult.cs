using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VitalChoice.Core.Infrastructure
{
	public class ForbiddenResult : StatusCodeResult
	{
		public ForbiddenResult()
			: base(StatusCodes.Status403Forbidden)
		{
		}
	}
}
