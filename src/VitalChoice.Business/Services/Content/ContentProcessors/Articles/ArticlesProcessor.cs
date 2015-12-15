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

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticlesProcessor : ContentProcessor<TtlShortArticleListModel, ArticleParameters, ContentCategory>
    {
        private readonly IArticleService _articleService;

        public ArticlesProcessor(IObjectMapper<ArticleParameters> mapper,
            IArticleService articleService) : base(mapper)
        {
            _articleService = articleService;
        }

        protected override async Task<TtlShortArticleListModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid category");
            }

            var page = viewContext.Parameters.Page != 0 ? viewContext.Parameters.Page : 1;
            ArticleItemListFilter filter = new ArticleItemListFilter();
            filter.CategoryId = viewContext.Entity.ParentId.HasValue ? (int?)viewContext.Entity.Id : null;
            filter.Paging.PageIndex = page;
            filter.Paging.PageItemCount = ContentConstants.ARTICLES_LIST_TAKE_COUNT;
            filter.Sorting.Path = ArticleSortPath.PublishedDate;
            filter.Sorting.SortOrder = SortOrder.Desc;
            var data = await _articleService.GetArticlesAsync(filter);

            TtlShortArticleListModel toReturn = new TtlShortArticleListModel()
            {
                Items = data.Items.Select(p => new TtlShortArticleModel()
                {
                    Name = p.Name,
                    Url = ContentConstants.ARTICLE_BASE_URL + p.Url,
                    PublishedDate = p.PublishedDate,
                }).ToList(),
                Count = data.Count,
            };

            if (page > 1)
            {
                toReturn.PreviousLink = String.Format("{0}{1}?{2}={3}", ContentConstants.ARTICLE_CATEGORY_BASE_URL, viewContext.Parameters.Url,
                    QueryStringConstants.PAGE, page-1);
                if(viewContext.Parameters.Preview)
                {
                    toReturn.PreviousLink += String.Format("&{0}=true", QueryStringConstants.PREVIEW);
                }
            }
            if (toReturn.Count> page* ContentConstants.ARTICLES_LIST_TAKE_COUNT)
            {
                toReturn.NextLink = String.Format("{0}{1}?{2}={3}", ContentConstants.ARTICLE_CATEGORY_BASE_URL, viewContext.Parameters.Url,
                    QueryStringConstants.PAGE, page+1);
                if (viewContext.Parameters.Preview)
                {
                    toReturn.NextLink += String.Format("&{0}=true", QueryStringConstants.PREVIEW);
                }
            }

            return toReturn;
        }

        public override string ResultName => "Articles";
    }
}