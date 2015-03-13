using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Content;
using System.Threading.Tasks;
using VitalChoice.Public.Models;

namespace VitalChoice.Public.Content.Controllers
{
    public class BaseContentController : Controller
    {
        protected readonly IContentService contentService;

        public BaseContentController(IContentService contentService)
        {
            this.contentService = contentService;
        }

        public virtual ViewResult BaseView(ContentPageViewModel model)
        {
            return View("~/Views/Content/ContentPage.cshtml", model);
        }
    }
}