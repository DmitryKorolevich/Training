using Microsoft.AspNet.Mvc;
using VitalChoice.Core.GlobalFilters;

namespace VitalChoice.Core.Base
{
    [MvcExceptionFilter]
    [SetAffiliateCookieFilter]
    public abstract class BaseMvcController : BaseController
    {
    }
}