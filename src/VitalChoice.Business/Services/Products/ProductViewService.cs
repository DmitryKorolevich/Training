using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Products
{
    public class ProductViewForCustomerModel : ContentParametersModel
    {
        public IList<CustomerTypeCode> CustomerTypeCodes { get; set; }
    }

    public class ProductViewService : ContentViewService<ProductContent, ProductViewForCustomerModel>, IProductViewService
    {
        private readonly IProductService _productService;

        public ProductViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ProductContent> contentRepository,
            IObjectMapper<ProductViewForCustomerModel> mapper, IObjectMapperFactory mapperFactory, IProductService productService)
            : base(templatesCache, loggerProvider.CreateLoggerDefault(), processorService, contentRepository, mapper, mapperFactory)
        {
            _productService = productService;
        }

        private static IList<CustomerTypeCode> GetCategoryMenuAvailability(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated
                ? (user.IsInRole(IdentityConstants.WholesaleCustomer)
                    ? new List<CustomerTypeCode>() {CustomerTypeCode.Wholesale, CustomerTypeCode.All}
                    : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All})
                : new List<CustomerTypeCode>() {CustomerTypeCode.Retail, CustomerTypeCode.All};
            //todo: refactor when authentication mechanism gets ready
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
                    : String.Format(ContentConstants.CONTENT_PAGE_TITLE_GENERAL_FORMAT, productDynamic.Name);
            }
            return result;
        }

        protected override async Task<ProductContent> GetDataInternal(ProductViewForCustomerModel model, ContentViewContext viewContext)
        {
            var entity = await base.GetDataInternal(model, viewContext);

            //NOTE: Set Parameters for processors and CreateResult here.
            if (entity != null)
            {
                viewContext.Parameters.CustomerTypeCodes = GetCategoryMenuAvailability(viewContext.User);
                viewContext.Parameters.Product = await _productService.SelectAsync(entity.Id, true);
            }

            return entity;
        }
    }
}