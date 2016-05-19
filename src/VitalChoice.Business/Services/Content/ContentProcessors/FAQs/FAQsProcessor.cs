using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.FAQs;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors.FAQs
{
    public class FAQsProcessor : ContentProcessor<ICollection<TtlShortFAQModel>, FAQParameters, ContentCategory>
    {
        private readonly IFAQService _faqService;

        public FAQsProcessor(IObjectMapper<FAQParameters> mapper,
            IFAQService faqService) : base(mapper)
        {
            _faqService = faqService;
        }

        protected override async Task<ICollection<TtlShortFAQModel>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid category");
            }

            List<TtlShortFAQModel> toReturn = new List<TtlShortFAQModel>();
            if (viewContext.Entity.ParentId.HasValue)
            {
                FAQListFilter filter = new FAQListFilter();
                filter.CategoryId = viewContext.Entity.Id;
                filter.Sorting.Path = FAQSortPath.Created;
                filter.Sorting.SortOrder = SortOrder.Desc;
                var data = await _faqService.GetFAQsAsync(filter);

                toReturn = data.Items.Select(p => new TtlShortFAQModel()
                {
                    Name = p.Name,
                    Url = ContentConstants.FAQ_BASE_URL + p.Url,
                }).ToList();
            }

            return toReturn;
        }

        public override string ResultName => "FAQs";
    }
}