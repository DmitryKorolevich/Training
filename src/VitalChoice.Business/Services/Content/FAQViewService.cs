using System;
using System.Collections.Generic;
using System.Linq;
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
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Data.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content
{
    public class FAQViewService : ContentViewService<FAQ, ContentParametersModel>, IFAQViewService
    {
        private readonly IRecipeService _recipeService;

        public FAQViewService(ITtlGlobalCache templatesCache,
            ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService,
            IRepositoryAsync<FAQ> contentRepository,
            IObjectMapper<ContentParametersModel> mapper,
            IObjectMapperFactory mapperFactory,
            IRecipeService recipeService)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
            _recipeService = recipeService;
        }
    }
}
