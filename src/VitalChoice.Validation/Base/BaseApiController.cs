using VitalChoice.Validation.Helpers.GlobalFilters;

namespace VitalChoice.Validation.Base
{
	[BuildNumberValidationFilter]
    [ApiModelAutoValidationFilter]
    [ApiExceptionFilter]
    public abstract class BaseApiController : BaseController
    {
    }
}