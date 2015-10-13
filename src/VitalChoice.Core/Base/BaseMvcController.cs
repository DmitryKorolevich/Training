using Microsoft.AspNet.Mvc;
using VitalChoice.Core.GlobalFilters;

namespace VitalChoice.Core.Base
{
    [MvcExceptionFilter]
    public abstract class BaseMvcController : BaseController
    {
    }
}