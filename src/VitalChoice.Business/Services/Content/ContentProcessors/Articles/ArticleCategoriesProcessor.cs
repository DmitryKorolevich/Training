using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Articles
{
    public class ArticleCategoriesProcessor : ContentProcessor<TtlArticleCategoryModel, ArticleCategoryParameters, ContentCategory>
    {
        private readonly ICategoryService _categoryService;

        public ArticleCategoriesProcessor(IObjectMapper<ArticleCategoryParameters> mapper,
            ICategoryService categoryService) : base(mapper)
        {
            _categoryService = categoryService;
        }

        protected override async Task<TtlArticleCategoryModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid category");
            }

            var targetStatuses = new List<RecordStatusCode>() { RecordStatusCode.Active };
            if (viewContext.Entity.StatusCode == RecordStatusCode.NotActive)
            {
                if (!viewContext.Parameters.Preview)
                {
					throw new ApiException("Category not found", HttpStatusCode.NotFound);
				}
                targetStatuses.Add(RecordStatusCode.NotActive);
            }

            var rootCategory = await _categoryService.GetCategoriesTreeAsync(new CategoryTreeFilter
                    {
                        Type=ContentType.ArticleCategory,
                    });

            return PopulateCategoryTemplateModel(viewContext.Entity);
        }


        private TtlArticleCategoryModel PopulateCategoryTemplateModel(ContentCategory categoryContent)
        {
            var toReturn = new TtlArticleCategoryModel
            {
                Name = categoryContent.Name,
                Url = ArticleCategoryParameters.ArticleCategoryBaseUrl + categoryContent.Url,
                SubCategories = categoryContent.SubCategories?.Select(x => PopulateCategoryTemplateModel(x)).ToList(),
            };

            return toReturn;
        }

        public override string ResultName => "ArticleCategories";
    }
}