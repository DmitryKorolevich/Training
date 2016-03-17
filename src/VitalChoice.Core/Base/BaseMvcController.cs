using Microsoft.AspNet.Mvc;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Core.Base
{
    [MvcExceptionFilter]
    [SetAffiliateCookieFilter]
    public abstract class BaseMvcController : BaseController
    {
        private readonly IPageResultService _pageResultService;

        protected BaseMvcController(IPageResultService pageResultService)
        {
            _pageResultService = pageResultService;
        }

        public virtual IActionResult BaseNotFoundView()
		{
		    return _pageResultService.GetResult(PageResult.NotFound);
		}

		public virtual IActionResult GetItemNotAccessibleResult()
		{
			return _pageResultService.GetResult(PageResult.Forbidden);
        }
	}
}