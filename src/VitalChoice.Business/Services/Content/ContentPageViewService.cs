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
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Ecommerce.Domain.Exceptions;
using System.Net;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content
{
    public class ContentPageViewService : ContentViewService<ContentPage, ContentParametersModel>, IContentPageViewService
    {

        public ContentPageViewService(ITtlGlobalCache templatesCache, ILoggerFactory loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ContentPage> contentRepository,
            IObjectMapper<ContentParametersModel> mapper, IObjectMapperFactory mapperFactory,
            IOptions<AppOptions> appOptions)
            : base(templatesCache, loggerProvider.CreateLogger<ContentPageViewService>(), processorService, contentRepository, mapper, mapperFactory, appOptions)
        {
        }

        protected override async Task<ContentPage> GetDataInternal(ContentParametersModel model, ContentViewContext viewContext)
        {
            ContentPage entity = await base.GetDataInternal(model, viewContext);

            if (entity!=null && viewContext != null)
            {
                if (entity.StatusCode == RecordStatusCode.NotActive)
                {
                    if (!model.Preview)
                    {
                        throw new ApiException("Content page not found", HttpStatusCode.NotFound);
                    }
                }

                var customerTypes = GetItemAvailability(viewContext.User);
                if(!customerTypes.Contains(entity.Assigned))
                {
                    throw new ApiException("Content page not found", HttpStatusCode.NotFound);
                }
            }
            return entity;
        }

        private static IList<CustomerTypeCode> GetItemAvailability(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated
                ? (user.IsInRole(IdentityConstants.WholesaleCustomer)
                    ? new List<CustomerTypeCode>() { CustomerTypeCode.Wholesale, CustomerTypeCode.All }
                    : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All })
                : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All };
            //todo: refactor when authentication mechanism gets ready
        }
    }
}