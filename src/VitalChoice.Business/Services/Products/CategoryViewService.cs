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
using VitalChoice.ObjectMapping.Interfaces;

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

        protected override async Task<ProductCategoryContent> GetDataInternal(ProductViewForCustomerModel model,
            ContentViewContext viewContext)
        {
            ProductCategoryContent entity;
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                entity = await base.GetDataInternal(model, viewContext);
                viewContext.Parameters.CustomerTypeCode = GetCategoryMenuAvailability(viewContext.User);
                if (entity != null)
                {
                    entity.ProductCategory =
                        await
                            _productCategoryEcommerceRepository.Query(c => c.Id == entity.Id)
                                .SelectFirstOrDefaultAsync(false);
                }
                return entity;
            }

            var categoryEcommerce = await
                _productCategoryEcommerceRepository.Query(p => !p.ParentId.HasValue)
                    .SelectFirstOrDefaultAsync(false);
            if (categoryEcommerce == null)
                return null;
            entity =
                await
                    BuildQuery(ContentRepository.Query(p => p.Id == categoryEcommerce.Id))
                        .SelectFirstOrDefaultAsync(false);
            viewContext.Parameters.CustomerTypeCode = GetCategoryMenuAvailability(viewContext.User);
            return entity;
        }

        #endregion
	}
}
