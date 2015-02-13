using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validation.Helpers.GlobalFilters;

namespace VitalChoice.Validation.Controllers
{
    [ApiModelAutoValidationFilter]
    public abstract class BaseApiController : BaseController
    {
    }
}