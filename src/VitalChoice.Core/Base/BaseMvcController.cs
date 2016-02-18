using Microsoft.AspNet.Mvc;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Core.Base
{
    [MvcExceptionFilter]
    [SetAffiliateCookieFilter]
    public abstract class BaseMvcController : BaseController
    {
		public virtual IActionResult BaseNotFoundView()
		{
			return Redirect("/content/" + ContentConstants.NOT_FOUND_PAGE_URL);
		}

		public virtual IActionResult GetItemNotAccessibleResult()
		{
			return Redirect("/content/" + ContentConstants.ACESS_DENIED_PAGE_URL);
		}
	}
}