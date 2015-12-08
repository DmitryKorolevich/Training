using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using Microsoft.Extensions.Logging;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class ContentPageViewService : ContentViewService<ContentPage, ContentServiceModel>, IContentPageViewService
    {
        public ContentPageViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ContentPage> contentRepository,
            IObjectMapper<ContentServiceModel> mapper) : base(templatesCache, loggerProvider, processorService, contentRepository, mapper)
        {
        }

        #region Public

        #endregion
    }
}