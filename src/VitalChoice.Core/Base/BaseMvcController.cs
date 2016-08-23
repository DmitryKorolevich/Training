using Microsoft.AspNetCore.Mvc;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VitalChoice.Core.Base
{
    [MvcExceptionFilter]
    [SetAffiliateCookieFilter]
    public abstract class BaseMvcController : BaseController
    {
        public virtual IActionResult BaseNotFoundView()
        {
            return new NotFoundResult();
        }

		public virtual IActionResult GetItemNotAccessibleResult()
		{
			return new ForbiddenResult();
        }
	}
}