using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
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
using VitalChoice.Infrastructure.Domain.Transfer.TemplateModels.Recipes;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors.Recipes
{
    public class RecipeCategoriesProcessor : ContentProcessor<TtlRecipeCategoriesModel, RecipeParameters, ContentCategory>
    {
        public const string RecipeVideosChefCategoryName= "Recipe Videos by Chef";
        public const string RecipeVideosChefOtherCategoryName = "Others";

        private readonly ICategoryService _categoryService;
        private readonly IRecipeService _recipeService;

        public RecipeCategoriesProcessor(
            IObjectMapper<RecipeParameters> mapper,
            ICategoryService categoryService,
            IRecipeService recipeService) : base(mapper)
        {
            _categoryService = categoryService;
            _recipeService = recipeService;
        }

        protected override async Task<TtlRecipeCategoriesModel> ExecuteAsync(ProcessorViewContext viewContext)
        {
            if (viewContext.Entity == null)
            {
                throw new ApiException("Invalid category");
            }

            var rootCategory = await _categoryService.GetCategoriesTreeAsync(new CategoryTreeFilter
                    {
                        Type=ContentType.RecipeCategory,
                    });

            var data = PopulateCategoryTemplateModel(rootCategory);

            TtlRecipeCategoriesModel toReturn = new TtlRecipeCategoriesModel {
                AllCategories = data.SubCategories,
            };
            if(viewContext.Entity.ParentId.HasValue)
            {
                var category = FindCategory(toReturn.AllCategories, (p)=>p.Id == viewContext.Entity.Id);
                if(category!=null)
                {
                    toReturn.SubCategories = category.SubCategories;
                }
            }
            
            var rootChefCategory = FindCategory(toReturn.AllCategories, (p) => p.Name.ToLower() == RecipeVideosChefCategoryName.ToLower());
            if (rootChefCategory != null)
            {
                foreach (var category in rootChefCategory.SubCategories)
                {
                    if (category.Name.ToLower() != RecipeVideosChefOtherCategoryName.ToLower())
                    {
                        RecipeListFilter filter = new RecipeListFilter();
                        filter.StatusCode = RecordStatusCode.Active;
                        filter.CategoryId = category.Id;
                        filter.Sorting.Path = RecipeSortPath.Created;
                        filter.Sorting.SortOrder = FilterSortOrder.Desc;
                        var recipes = await _recipeService.GetRecipesAsync(filter);
                        category.Recipes = recipes.Items.Select(p => new TtlShortRecipeModel() {
                            Name=p.Name,
                            Url= ContentConstants.RECIPE_BASE_URL + p.Url,
                        }).ToList();
                        toReturn.ChefCategories.Add(category);
                    }
                }
            }

            return toReturn;
        }


        private TtlRecipeCategoryModel PopulateCategoryTemplateModel(ContentCategory categoryContent)
        {
            var toReturn = new TtlRecipeCategoryModel
            {
                Id = categoryContent.Id,
                Name = categoryContent.Name,
                Url = ContentConstants.RECIPE_CATEGORY_BASE_URL + categoryContent.Url,
                SubCategories = categoryContent.SubCategories?.Select(PopulateCategoryTemplateModel).ToList(),
            };

            return toReturn;
        }

        private TtlRecipeCategoryModel FindCategory(ICollection<TtlRecipeCategoryModel> categories, Func<TtlRecipeCategoryModel, bool> check)
        {
            TtlRecipeCategoryModel toReturn = null;
            if(categories!=null)
            {
                foreach(var category in categories)
                {
                    toReturn = FindCategory(category.SubCategories, check);
                    if(toReturn!=null)
                    {
                        break;
                    }
                    if(check(category))
                    { 
                        toReturn = category;
                        break;
                    }
                }
            }
            return toReturn;
        }

        public override string ResultName => "RecipeCategories";
    }
}