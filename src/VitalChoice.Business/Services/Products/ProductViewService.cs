using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
	public class ProductViewService : ContentViewService<ProductCategoryContent>, IProductViewService
	{
        public ProductViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
	        ProductCategoryDefaultProcessor defaultProcessor,
	        IContentProcessorService processorService)
	        : base(templatesCache, loggerProvider, defaultProcessor, processorService)
        {
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
