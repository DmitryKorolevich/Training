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
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
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
    public class RecentArticlesForContentPageProcessor : ContentProcessor<ICollection<TtlShortArticleModel>, ArticleParameters, ContentPage>
    {
        private readonly IArticleService _articleService;

        public RecentArticlesForContentPageProcessor(IObjectMapper<ArticleParameters> mapper,
            IArticleService articleService) : base(mapper)
        {
            _articleService = articleService;
        }

        protected override async Task<ICollection<TtlShortArticleModel>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            ArticleItemListFilter filter = new ArticleItemListFilter();
            filter.ExcludeIds = new List<int>() {
                viewContext.Entity.Id
            };
            filter.StatusCode=RecordStatusCode.Active;
            filter.Paging.PageItemCount = ContentConstants.RECENT_ARTICLES_LIST_TAKE_COUNT;
            filter.Sorting.Path = ArticleSortPath.PublishedDate;
            filter.Sorting.SortOrder = FilterSortOrder.Desc;
            var data = await _articleService.GetArticlesAsync(filter);

            var toReturn = new List<TtlShortArticleModel>(data.Items.Select(p => new TtlShortArticleModel()
            {
                Name = p.Name,
                Url = ContentConstants.ARTICLE_BASE_URL + p.Url,
                PublishedDate = p.PublishedDate,
            }).ToList());

            return toReturn;
        }

        public override string ResultName => "RecentArticles";
    }
}