using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Controllers.Content
{
    public class FaqController : BaseContentController
    {
        private readonly IFAQCategoryViewService _faqCategoryViewService;
        private readonly IFAQViewService _faqViewService;

        public FaqController(
            IFAQCategoryViewService faqCategoryViewService,
            IFAQViewService faqViewService,
            IPageResultService pageResultService) : base(pageResultService)
        {
            _faqCategoryViewService = faqCategoryViewService;
            _faqViewService = faqViewService;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _faqCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _faqCategoryViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> FAQ(string url)
        {
            var toReturn = await _faqViewService.GetContentAsync(ActionContext, BindingContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }
    }
}