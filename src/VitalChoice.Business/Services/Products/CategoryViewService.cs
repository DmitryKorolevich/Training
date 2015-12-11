using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Templates;
using Templates.Exceptions;
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
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Identity;

namespace VitalChoice.Business.Services.Products
{
    public class CategoryViewService : ContentViewService<ProductCategoryContent, ProductViewForCustomerModel>, ICategoryViewService
	{

        private readonly IEcommerceRepositoryAsync<ProductCategory> _productCategoryEcommerceRepository;

        public CategoryViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ProductCategoryContent> contentRepository,
            IObjectMapper<ProductViewForCustomerModel> mapper, IObjectMapperFactory mapperFactory,
            IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
            _productCategoryEcommerceRepository = productCategoryEcommerceRepository;
        }

        #region Public

        private static IList<CustomerTypeCode> GetCategoryMenuAvailability(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated
                ? (user.IsInRole(IdentityConstants.WholesaleCustomer)
                    ? new List<CustomerTypeCode>() { CustomerTypeCode.Wholesale, CustomerTypeCode.All }
                    : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All })
                : new List<CustomerTypeCode>() { CustomerTypeCode.Retail, CustomerTypeCode.All };
            //todo: refactor when authentication mechanism gets ready
        }

        protected override async Task<ContentViewContext<ProductCategoryContent>> GetDataInternal(ProductViewForCustomerModel model,
            IDictionary<string, object> parameters, ClaimsPrincipal user)
        {
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                var baseContext = await base.GetDataInternal(model, parameters, user);
                baseContext.Parameters.CustomerTypeCode = GetCategoryMenuAvailability(user);
                if (baseContext.Entity != null)
                {
                    baseContext.Entity.ProductCategory =
                        await
                            _productCategoryEcommerceRepository.Query(c => c.Id == baseContext.Entity.Id)
                                .SelectFirstOrDefaultAsync(false);
                }
                return baseContext;
            }

            var categoryEcommerce = await
                _productCategoryEcommerceRepository.Query(p => !p.ParentId.HasValue)
                    .SelectFirstOrDefaultAsync(false);
            if (categoryEcommerce == null)
                return new ContentViewContext<ProductCategoryContent>(parameters, null, user);
            var productCategory =
                await
                    BuildQuery(ContentRepository.Query(p => p.Id == categoryEcommerce.Id))
                        .SelectFirstOrDefaultAsync(false);
            productCategory.ProductCategory = categoryEcommerce;
            var context = new ContentViewContext<ProductCategoryContent>(parameters, productCategory, user);
            context.Parameters.CustomerTypeCode = GetCategoryMenuAvailability(user);
            return context;
        }

	    #endregion
	}
}
