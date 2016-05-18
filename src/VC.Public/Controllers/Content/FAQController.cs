using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ICategoryService _categoryService;
        private readonly IFAQService _fAQService;

        public FaqController(
            IFAQCategoryViewService faqCategoryViewService,
            IFAQViewService faqViewService,
            ICategoryService categoryService,
            IFAQService fAQService,
            IPageResultService pageResultService) : base(pageResultService)
        {
            _faqCategoryViewService = faqCategoryViewService;
            _faqViewService = faqViewService;
            _categoryService = categoryService;
            _fAQService = fAQService;
        }

        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var toReturn = await _faqCategoryViewService.GetContentAsync(ControllerContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string url)
        {
            var toReturn = await _faqCategoryViewService.GetContentAsync(ControllerContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> CategoryByIdOld([FromQuery]string idSLC)
        {
            int idold;
            if (Int32.TryParse(idSLC, out idold))
            {
                var item = await _categoryService.GetCategoryByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url) && item?.Type == ContentType.FaqCategory)
                {
                    return RedirectPermanent($"/faqs/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> FAQ(string url)
        {
            var toReturn = await _faqViewService.GetContentAsync(ControllerContext, User);
            if (toReturn?.Body != null)
            {
                return BaseView(new ContentPageViewModel(toReturn));
            }
            return BaseNotFoundView();
        }

        [HttpGet]
        public async Task<IActionResult> FAQByIdOld([FromQuery]string idFAQ)
        {
            int idold;
            if (Int32.TryParse(idFAQ, out idold))
            {
                var item = await _fAQService.GetFAQByIdOldAsync(idold);
                if (!string.IsNullOrEmpty(item?.Url))
                {
                    return RedirectPermanent($"/faq/{item.Url}");
                }
            }

            return BaseNotFoundView();
        }
    }
}