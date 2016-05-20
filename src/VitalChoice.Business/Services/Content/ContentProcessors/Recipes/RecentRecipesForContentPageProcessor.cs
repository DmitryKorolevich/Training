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
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
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
    public class RecentRecipesForContentPageProcessor : ContentProcessor<ICollection<TtlShortRecipeModel>, RecipeParameters, ContentPage>
    {
        private readonly IRecipeService _recipeService;

        public RecentRecipesForContentPageProcessor(IObjectMapper<RecipeParameters> mapper,
            IRecipeService recipeService) : base(mapper)
        {
            _recipeService = recipeService;
        }

        protected override async Task<ICollection<TtlShortRecipeModel>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            RecipeListFilter filter = new RecipeListFilter();
            filter.Paging.PageItemCount = ContentConstants.RECENT_RECIPES_FOR_ARTICLE_LIST_TAKE_COUNT;
            filter.Sorting.Path = RecipeSortPath.Created;
            filter.Sorting.SortOrder = FilterSortOrder.Desc;
            var data = await _recipeService.GetRecipesAsync(filter);

            var toReturn = new List<TtlShortRecipeModel>(data.Items.Select(p => new TtlShortRecipeModel()
            {
                Name = p.Name,
                Url = ContentConstants.RECIPE_BASE_URL + p.Url,
            }).ToList());

            return toReturn;
        }

        public override string ResultName => "RecentRecipes";
    }
}