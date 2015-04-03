using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Controllers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Validation.Helpers.GlobalFilters;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.Users;
using VitalChoice.Validation.Logic;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Core;
using VitalChoice.Admin.Models;

namespace VitalChoice.Admin.Controllers
{
    public class ExampleApiController : BaseApiController
    {
        [HttpPost]
        [ControlMode(UserCreateMode.CreateStandardUser, typeof(UserCreateSettings))]
        public Result<User> IndexPost([FromBody]UserCreateModel model)
        {
            (Settings.ValidationMode as UserCreateSettings).ShowAccountType = true;
            User user = ConvertWithValidate(model);
            if (user == null)
                return null;
            return user;
        }

        [HttpPost]
        [ControlMode(UserCreateMode.CreateAdmin, typeof(UserCreateSettings))]
        public Result<User> IndexPost2([FromBody]UserCreateModel model)
        {
            User user = ConvertWithValidate(model);
            if (user == null)
                return null;
            return user;
        }
    }
}