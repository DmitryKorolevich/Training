using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using Microsoft.Extensions.Logging;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Interfaces.Services.Content;
using System.Security.Claims;

namespace VitalChoice.Business.Services.Content
{
    public class ContentPageViewService : ContentViewService<ContentPage, ContentServiceModel>, IContentPageViewService
    {
        #region Public

        #endregion

        public ContentPageViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ContentPage> contentRepository,
            IObjectMapper<ContentServiceModel> mapper, IObjectMapperFactory mapperFactory)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
        }

        protected override async Task<ContentViewContext<ContentPage>> GetDataInternal(ContentServiceModel model,
            IDictionary<string, object> parameters, ClaimsPrincipal user)
        {
            ContentViewContext<ContentPage> context = await base.GetDataInternal(model, parameters, user);

            return context;
        }
    }
}