using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Helpers;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Transfer.TemplateModels;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
	public class ProductViewService : ContentViewService<ProductCategoryContent>, IProductViewService
	{
	    private readonly IEcommerceRepositoryAsync<ProductCategory> _productCategoryEcommerceRepository;

        public ProductViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
	        IContentProcessor<ProductCategoryContent, ProcessorModel> defaulProcessor,
	        IContentProcessorService processorService, IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository)
	        : base(templatesCache, loggerProvider, defaulProcessor, processorService)
        {
            _productCategoryEcommerceRepository = productCategoryEcommerceRepository;
        }

	    #region Public

	    protected override async Task<ProductCategoryContent> GetData(IDictionary<string, object> queryData)
	    {
	        var productCategory = await base.GetData(queryData);
            if (productCategory == null)
            {
                Logger.LogInformation("The category could not be found {" + queryData.FormatDictionary() + "}");
                //return explicitly null to see the real result of operation and don't look over code above regarding the real value
                return null;
            }
            if (productCategory.ContentItem == null)
            {
                Logger.LogError("The category {0} have no template", productCategory.Url);
                return null;
            }
            return productCategory;
	    }

	    public Task<ContentViewModel> GetProductCategoryContentAsync(IList<CustomerTypeCode> customerTypeCodes,
	        Dictionary<string, object> parameters)
	    {
	        var paramProperty =
	            typeof (ProductCategoryParameters)
	                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
	                .FirstOrDefault(p => p.PropertyType == typeof (IList<CustomerTypeCode>));
	        if (paramProperty != null)
	        {
	            parameters.Add(paramProperty.Name, customerTypeCodes);
	        }
	        return GetContentAsync(parameters);
	    }

	    #endregion
    }
}
