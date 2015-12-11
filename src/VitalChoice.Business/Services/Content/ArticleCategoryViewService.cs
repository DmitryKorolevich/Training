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

namespace VitalChoice.Business.Services.Content
{
    public class ArticleCategoryViewService : ContentViewService<ContentCategory, CategoryContentServiceModel>, IArticleCategoryViewService
    {
        private readonly ICategoryService _categoryService;

        public ArticleCategoryViewService(ITtlGlobalCache templatesCache,
            ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService,
            IRepositoryAsync<ContentCategory> contentRepository,
            IObjectMapper<CategoryContentServiceModel> mapper,
            IObjectMapperFactory mapperFactory,
            ICategoryService categoryService)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
            _categoryService = categoryService;
        }

        #region Public

        protected override Expression<Func<ContentCategory, bool>> FilterExpression(CategoryContentServiceModel model) =>
            p => (p.Url == model.Url || (model.IdRootCategory.HasValue && p.Id==model.IdRootCategory.Value)) && p.StatusCode != RecordStatusCode.Deleted && p.Type==ContentType.ArticleCategory;

        protected override async Task<ContentViewContext<ContentCategory>> GetDataInternal(CategoryContentServiceModel model,
            IDictionary<string, object> parameters, ClaimsPrincipal user)
        {
            ContentViewContext<ContentCategory> context;
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                context = await base.GetDataInternal(model, parameters, user);
            }

            model.IdRootCategory = await _categoryService.GetIdRootCategoryAsync(ContentType.ArticleCategory);
            context = await base.GetDataInternal(model, parameters, user);

            if (context != null)
            {
                if (parameters.ContainsKey(QueryStringConstants.ID_CATEGORY) && parameters[QueryStringConstants.ID_CATEGORY] is string)
                {
                    int id = 0;
                    if(Int32.TryParse((string)parameters[QueryStringConstants.ID_CATEGORY], out id))
                    {
                        context.Parameters.IdCategory = id;
                    }
                }
                if (parameters.ContainsKey(QueryStringConstants.PAGE) && parameters[QueryStringConstants.PAGE] is string)
                {
                    int id = 0;
                    if (Int32.TryParse((string)parameters[QueryStringConstants.PAGE], out id))
                    {
                        context.Parameters.ArticlesPageIndex = id;
                    }
                }
            }

            return context;
        }

	    #endregion
	}
}
