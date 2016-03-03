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
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content
{
    public class FAQCategoryViewService : ContentViewService<ContentCategory, CategoryContentParametersModel>, IFAQCategoryViewService
    {
        public FAQCategoryViewService(ITtlGlobalCache templatesCache,
            ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService,
            IRepositoryAsync<ContentCategory> contentRepository,
            IObjectMapper<CategoryContentParametersModel> mapper,
            IObjectMapperFactory mapperFactory)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
        }

        #region Public

        protected override Expression<Func<ContentCategory, bool>> FilterExpression(CategoryContentParametersModel model) =>
            p => p.Url == model.Url && p.StatusCode != RecordStatusCode.Deleted && p.Type==ContentType.FaqCategory;

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
                        BuildQuery(ContentRepository.Query(p => !p.ParentId.HasValue && p.Type == ContentType.FaqCategory))
                            .SelectFirstOrDefaultAsync(false);
            }

            if (viewContext != null)
            {
                if (entity != null && entity.StatusCode == RecordStatusCode.NotActive)
                {
                    if (!model.Preview)
                    {
                        throw new ApiException("Category not found", HttpStatusCode.NotFound);
                    }
                }
            }
            return entity;
        }

        #endregion
	}
}
