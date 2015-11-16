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

namespace VitalChoice.Business.Services.Products
{
	public class ProductViewService : ContentViewService<ProductContent>, IProductViewService
	{
        public ProductViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
	        IContentProcessor<ProductContent, ProcessorModel> defaultProcessor,
	        IContentProcessorService processorService)
	        : base(templatesCache, loggerProvider, defaultProcessor, processorService)
        {
        }

	    #region Public

	    protected override async Task<ProductContent> GetData(IDictionary<string, object> queryData)
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

	    public Task<ContentViewModel> GetProductPageContentAsync(IList<CustomerTypeCode> customerTypeCodes,
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