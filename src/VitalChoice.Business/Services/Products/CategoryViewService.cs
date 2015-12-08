using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Templates;
using Templates.Exceptions;
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
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Business.Services.Products
{
    public class CategoryViewService : ContentViewService<ProductCategoryContent, ProductViewForCustomerModel>, ICategoryViewService
	{

        private readonly IEcommerceRepositoryAsync<ProductCategory> _productCategoryEcommerceRepository;

        public CategoryViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ProductCategoryContent> contentRepository,
            IObjectMapper<ProductViewForCustomerModel> mapper, IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository)
            : base(templatesCache, loggerProvider, processorService, contentRepository, mapper)
        {
            _productCategoryEcommerceRepository = productCategoryEcommerceRepository;
        }

        #region Public

        protected override async Task<ContentViewContext<ProductCategoryContent>> GetDataInternal(ProductViewForCustomerModel model,
            IDictionary<string, object> parameters)
        {
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                var baseContext = await base.GetDataInternal(model, parameters);
                if (baseContext != null)
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
                return null;
            var productCategory =
                await
                    BuildQuery(ContentRepository.Query(p => p.Id == categoryEcommerce.Id))
                        .SelectFirstOrDefaultAsync(false);
            productCategory.ProductCategory = categoryEcommerce;
            return new ContentViewContext<ProductCategoryContent>(parameters, productCategory);
        }

        public Task<ContentViewModel> GetProductCategoryContentAsync(IList<CustomerTypeCode> customerTypeCodes,
	        Dictionary<string, object> parameters)
	    {
	        parameters.Add("CustomerTypeCodes", customerTypeCodes);
	        return GetContentAsync(parameters);
	    }

	    #endregion
	}
}
