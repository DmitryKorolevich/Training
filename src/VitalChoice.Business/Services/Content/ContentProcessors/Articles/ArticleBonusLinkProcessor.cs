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
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleBonusLinkProcessor : ContentProcessor<ArticleBonusLink, ArticleParameters, ContentDataItem>
    {
        private readonly IArticleService _articleService;

        public ArticleBonusLinkProcessor(IObjectMapper<ArticleParameters> mapper,
            IArticleService articleService) : base(mapper)
        {
            _articleService = articleService;
        }

        protected override async Task<ArticleBonusLink> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid data");
            }

            ArticleBonusLink toReturn = null;

            var links = await _articleService.GetArticleBonusLinksAsync();
            toReturn = links.Where(p => p.StartDate <= DateTime.Now).OrderByDescending(p=>p.StartDate).FirstOrDefault();

            return toReturn;
        }

        public override string ResultName => "ArticleBonusLink";
    }
}