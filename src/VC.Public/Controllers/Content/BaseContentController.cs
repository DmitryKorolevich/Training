using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models;
using VitalChoice.Core.Base;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Controllers.Content
{
    public class BaseContentController : BaseMvcController
	{
        //Get params fron action params and all aditional query params
        public virtual ViewResult BaseView(ContentPageViewModel model)
        {
            return View("~/Views/Content/ContentPage.cshtml", model);
        }
	}
}