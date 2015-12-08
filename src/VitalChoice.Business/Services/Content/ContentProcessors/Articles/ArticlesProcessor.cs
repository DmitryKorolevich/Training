using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticlesProcessor : ContentProcessor<PagedList<TtlShortArticleModel>, ArticleCategoryParameters, ContentCategory>
    {
        private readonly IArticleService _articleService;

        public ArticlesProcessor(IObjectMapper<ArticleCategoryParameters> mapper,
            IArticleService articleService) : base(mapper)
        {
            _articleService = articleService;
        }

        protected override async Task<PagedList<TtlShortArticleModel>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            ArticleItemListFilter filter = new ArticleItemListFilter();
            filter.CategoryId = viewContext.Parameters.IdCategory;
            if (viewContext.Parameters.ArticlesPageIndex.HasValue)
            {
                filter.Paging.PageIndex = viewContext.Parameters.ArticlesPageIndex.Value;
            }
            filter.Paging.PageItemCount = ContentConstants.ARTICLES_LIST_TAKE_COUNT;
            var data = await _articleService.GetArticlesAsync(filter);

            PagedList<TtlShortArticleModel> toReturn = new PagedList<TtlShortArticleModel>(data.Items.Select(p=>new TtlShortArticleModel()
            {
                Name=p.Name,
                Url= ArticleCategoryParameters.ArticleBaseUrl+ p.Url,
                PublishedDate=p.PublishedDate,
            }).ToList(), data.Count);

            return toReturn;
        }

        public override string ResultName => "Articles";
    }
}