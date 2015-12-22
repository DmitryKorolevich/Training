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
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Articles;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.FAQs;
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Content.ContentProcessors.FAQs
{
    public class FAQCategoriesProcessor : ContentProcessor<TtlFAQCategoriesModel, FAQParameters, ContentCategory>
    {
        private readonly ICategoryService _categoryService;

        public FAQCategoriesProcessor(
            IObjectMapper<FAQParameters> mapper,
            ICategoryService categoryService) : base(mapper)
        {
            _categoryService = categoryService;
        }

        protected override async Task<TtlFAQCategoriesModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid category");
            }

            var rootCategory = await _categoryService.GetCategoriesTreeAsync(new CategoryTreeFilter
                    {
                        Type=ContentType.FaqCategory,
                    });

            var data = PopulateCategoryTemplateModel(rootCategory);

            TtlFAQCategoriesModel toReturn = new TtlFAQCategoriesModel() {
                AllCategories = data.SubCategories,
            };

            if (viewContext.Entity.ParentId.HasValue)
            {
                var category = FindCategory(toReturn.AllCategories, (p) => p.Id == viewContext.Entity.Id);
                if (category != null)
                {
                    toReturn.SubCategories = category.SubCategories;
                }
            }

            return toReturn;
        }


        private TtlFAQCategoryModel PopulateCategoryTemplateModel(ContentCategory categoryContent)
        {
            var toReturn = new TtlFAQCategoryModel
            {
                Id = categoryContent.Id,
                Name = categoryContent.Name,
                Url = ContentConstants.FAQ_CATEGORY_BASE_URL + categoryContent.Url,
                SubCategories = categoryContent.SubCategories?.Select(PopulateCategoryTemplateModel).ToList(),
            };

            return toReturn;
        }

        private TtlFAQCategoryModel FindCategory(ICollection<TtlFAQCategoryModel> categories, Func<TtlFAQCategoryModel, bool> check)
        {
            TtlFAQCategoryModel toReturn = null;
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    toReturn = FindCategory(category.SubCategories, check);
                    if (toReturn != null)
                    {
                        break;
                    }
                    if (check(category))
                    {
                        toReturn = category;
                        break;
                    }
                }
            }
            return toReturn;
        }

        public override string ResultName => "FAQCategories";
    }
}