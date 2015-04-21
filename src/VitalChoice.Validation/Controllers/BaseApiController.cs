using VitalChoice.Validation.Helpers.GlobalFilters;

namespace VitalChoice.Validation.Controllers
{
    [ApiModelAutoValidationFilter]
    [ApiExceptionFilter]
    public abstract class BaseApiController : BaseController
    {
    }
}