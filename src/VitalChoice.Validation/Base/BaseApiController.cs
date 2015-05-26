using VitalChoice.Validation.Helpers.GlobalFilters;

namespace VitalChoice.Validation.Base
{
    [ApiModelAutoValidationFilter]
    [ApiExceptionFilter]
    public abstract class BaseApiController : BaseController
    {
    }
}