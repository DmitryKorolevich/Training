using VitalChoice.Infrastructure.GlobalFilters;

namespace VitalChoice.Infrastructure.Base
{
	[BuildNumberValidationFilter]
    [ApiModelAutoValidationFilter]
    [ApiExceptionFilter]
    public abstract class BaseApiController : BaseController
    {
    }
}