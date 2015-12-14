using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Ecommerce.Domain.Exceptions;
using System.Net;
using Microsoft.AspNet.Mvc;

namespace VitalChoice.Business.Services.Content
{
    public class ArticleCategoryViewService : ContentViewService<ContentCategory, CategoryContentParametersModel>, IArticleCategoryViewService
    {
        private readonly ICategoryService _categoryService;

        public ArticleCategoryViewService(ITtlGlobalCache templatesCache,
            ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService,
            IRepositoryAsync<ContentCategory> contentRepository,
            IObjectMapper<CategoryContentParametersModel> mapper,
            IObjectMapperFactory mapperFactory,
            ICategoryService categoryService)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
            _categoryService = categoryService;
        }

        #region Public

        protected override Expression<Func<ContentCategory, bool>> FilterExpression(CategoryContentParametersModel model) =>
            p => p.Url == model.Url && p.StatusCode != RecordStatusCode.Deleted && p.Type==ContentType.ArticleCategory;

        protected override async Task<ContentCategory> GetDataInternal(CategoryContentParametersModel model, ContentViewContext viewContext)
        {
            ContentCategory entity;

            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                entity = await base.GetDataInternal(model, viewContext);
            }
            else
            {
                entity =
                    await
                        BuildQuery(ContentRepository.Query(p => !p.ParentId.HasValue && p.Type == ContentType.ArticleCategory))
                            .SelectFirstOrDefaultAsync(false);
            }

            if (viewContext != null)
            {
                var targetStatuses = new List<RecordStatusCode> {RecordStatusCode.Active};
                if (entity != null && entity.StatusCode == RecordStatusCode.NotActive)
                {
                    if (!model.Preview)
                    {
                        throw new ApiException("Category not found", HttpStatusCode.NotFound);
                    }
                    targetStatuses.Add(RecordStatusCode.NotActive);
                }
            }
            return entity;
        }

        #endregion
	}
}
