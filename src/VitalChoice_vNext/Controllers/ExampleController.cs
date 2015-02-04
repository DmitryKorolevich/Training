using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Controllers;
using VitalChoice.Models;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Validators.Users;

namespace VitalChoice.Controllers
{
    public class ExampleApiController : BaseController
    { 
        [HttpPost]
        [ControlMode(UserCreateMode.CreateStandardUser, typeof(UserCreateSettings))]
        public JsonResult Index(UserCreateModel model)
        {
            User user = ConvertWithValidate(model);
            if (user == null)
                return null;
            return Json(user);
        }

        [HttpPost]
        [ControlMode(UserCreateMode.CreateAdmin, typeof(UserCreateSettings))]
        public JsonResult Index2(UserCreateModel model)
        {
            User user = ConvertWithValidate(model);
            if (user == null)
                return null;
            return Json(user);
        }
    }
}