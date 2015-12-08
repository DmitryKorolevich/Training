using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Services.Products
{
    public class ProductViewForCustomerModel : ContentServiceModel
    {
        public IList<CustomerTypeCode> CustomerTypeCodes { get; set; }
    }

    public class ProductViewService : ContentViewService<ProductContent, ProductViewForCustomerModel>, IProductViewService
    {
        private readonly IProductService _productService;

        public ProductViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ProductContent> contentRepository,
            IObjectMapper<ProductViewForCustomerModel> mapper, IProductService productService)
            : base(templatesCache, loggerProvider, processorService, contentRepository, mapper)
        {
            _productService = productService;
        }

        protected override ContentViewModel CreateResult(string generatedHtml, ContentViewContext<ProductContent> viewContext)
        {
            var result = base.CreateResult(generatedHtml, viewContext);
            var title = viewContext.Entity.ContentItem.Title;
            ProductDynamic productDynamic = viewContext.Parameters.Product;
            if (productDynamic != null)
            {
                result.Title = !string.IsNullOrWhiteSpace(title)
                    ? title
                    : $"{productDynamic.Name} | {productDynamic.Data.SubTitle}";
            }
            return result;
        }

        protected override async Task<ContentViewContext<ProductContent>> GetDataInternal(ProductViewForCustomerModel model,
            IDictionary<string, object> queryData)
        {
            var viewContext = await base.GetDataInternal(model, queryData);
            
            //NOTE: Set Parameters for processors and CreateResult here.
            viewContext.Parameters.Product = _productService.SelectAsync(viewContext.Entity.Id, true);

            return viewContext;
        }

        #region Public

        public Task<ContentViewModel> GetProductPageContentAsync(IList<CustomerTypeCode> customerTypeCodes,
            Dictionary<string, object> parameters)
        {
            parameters.Add("CustomerTypeCodes", customerTypeCodes);
            return GetContentAsync(parameters);
        }

        #endregion
    }
}