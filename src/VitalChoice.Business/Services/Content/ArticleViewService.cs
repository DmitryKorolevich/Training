using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content
{
    public class ArticleViewService : ContentViewService<Article, ContentParametersModel>, IArticleViewService
    {
        public ArticleViewService(ITtlGlobalCache templatesCache,
            ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService,
            IRepositoryAsync<Article> contentRepository,
            IObjectMapper<ContentParametersModel> mapper,
            IObjectMapperFactory mapperFactory)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
        }
	}
}
