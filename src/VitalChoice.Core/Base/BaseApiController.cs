using VitalChoice.Core.GlobalFilters;

namespace VitalChoice.Core.Base
{
	[BuildNumberValidationFilter]
    [ApiModelAutoValidationFilter]
    [ApiExceptionFilter]
    public abstract class BaseApiController : BaseController
    {
    }
}