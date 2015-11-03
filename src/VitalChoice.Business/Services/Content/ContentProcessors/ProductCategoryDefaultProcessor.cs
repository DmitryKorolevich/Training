using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class ProductCategoryDefaultProcessor : GetContentProcessor<ProductCategoryContent, ProcessorModel>
    {
        private readonly IEcommerceRepositoryAsync<ProductCategory> _productCategoryEcommerceRepository;

        public ProductCategoryDefaultProcessor(IObjectMapper<ProcessorModel> mapper,
            IRepositoryAsync<ProductCategoryContent> contentRepository,
            IEcommerceRepositoryAsync<ProductCategory> productCategoryEcommerceRepository)
            : base(mapper, contentRepository)
        {
            _productCategoryEcommerceRepository = productCategoryEcommerceRepository;
        }

        public override async Task<ProductCategoryContent> ExecuteAsync(ProcessorModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                var productCategory = await base.ExecuteAsync(model);
                if (productCategory == null)
                    return null;
                productCategory.ProductCategory =
                    await
                        _productCategoryEcommerceRepository.Query(c => c.Id == productCategory.Id)
                            .SelectFirstOrDefaultAsync(false);
                return productCategory;
            }
            else
            {
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
                return productCategory;
            }
        }
    }
}
