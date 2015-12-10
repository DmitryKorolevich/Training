﻿using System;
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
    public class RecentArticlesProcessor : ContentProcessor<ICollection<TtlShortArticleModel>, ArticleParameters, Article>
    {
        private readonly IArticleService _articleService;

        public RecentArticlesProcessor(IObjectMapper<ArticleParameters> mapper,
            IArticleService articleService) : base(mapper)
        {
            _articleService = articleService;
        }

        protected override async Task<ICollection<TtlShortArticleModel>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid article");
            }

            ArticleItemListFilter filter = new ArticleItemListFilter();
            filter.ExcludeIds = new List<int>() {
                viewContext.Entity.Id
            };
            filter.Paging.PageItemCount = ContentConstants.RECENT_ARTICLES_LIST_TAKE_COUNT;
            filter.Sorting.Path = ArticleSortPath.PublishedDate;
            filter.Sorting.SortOrder = SortOrder.Desc;
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