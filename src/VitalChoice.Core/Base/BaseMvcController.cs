using Microsoft.AspNet.Mvc;
using VitalChoice.Core.GlobalFilters;

namespace VitalChoice.Core.Base
{
	//[BuildNumberValidationFilter]
    //[ApiModelAutoValidationFilter]
    [MvcExceptionFilter]
    public abstract class BaseMvcController : BaseController
    {
    }
}