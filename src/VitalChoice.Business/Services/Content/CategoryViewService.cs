using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content
{
    public class CategoryViewService : ContentViewService<ProductCategoryContent, ProductViewForCustomerModel>, ICategoryViewService
	{

        private readonly IEcommerceRepositoryAsync<ProductCategory> _productCategoryEcommerceRepository;

        public CategoryViewService(ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService, IRepositoryAsync<ProductCategoryContent> contentRepository,
            IObjectMapper<ProductViewForCustomerModel> mapper, IObjectMapperFactory mapperFactory,
            IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository)
            : base(templatesCache, loggerProvider.CreateLogger<CategoryViewService>(), processorService, contentRepository, mapper, mapperFactory)
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


        protected override ContentViewModel CreateResult(string generatedHtml, ContentViewContext<ProductCategoryContent> viewContext)
        {
            var result = base.CreateResult(generatedHtml, viewContext);
            var title = viewContext.Entity.ContentItem.Title;
            ProductCategory category = viewContext.Entity?.ProductCategory;
            if(category.Id==0 && viewContext.Entity!=null)
            {
                //For root category
                category = (_productCategoryEcommerceRepository.Query(p => p.Id == viewContext.Entity.Id)
                    .Select(false)).FirstOrDefault();
            }
            if (category != null)
            {
                result.Title = !string.IsNullOrWhiteSpace(title)
                    ? title
                    : String.Format(ContentConstants.CONTENT_PAGE_TITLE_GENERAL_FORMAT, category.Name);
            }
            return result;
        }

        #endregion
    }
}
