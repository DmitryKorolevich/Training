using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Core.Base;

namespace VC.Public.Controllers.Content
{
    public class BaseContentController : BaseMvcController
	{
        //Get params fron action params and all aditional query params
        public virtual ViewResult BaseView(ContentPageViewModel model)
        {
            return View("~/Views/Content/ContentPage.cshtml", model);
        }

        public virtual ViewResult BaseNotFoundView()
        {
            var result = View("~/Views/Shared/Error404.cshtml");
            result.StatusCode = 404;
            return result;
        }
    }
}