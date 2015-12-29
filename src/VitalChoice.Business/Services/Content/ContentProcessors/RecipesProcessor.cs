using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeParameters
    {
        public int IdCategory { get; set; }
    }

    public class RecipesProcessor : ContentProcessor<List<Recipe>, RecipeParameters>
    {
        private readonly IReadRepositoryAsync<RecipeToContentCategory> _recipeToContentCategoryRepositoryAsync;

        public RecipesProcessor(IObjectMapper<RecipeParameters> mapper,
            IReadRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepositoryAsync) : base(mapper)
        {
            _recipeToContentCategoryRepositoryAsync = recipeToContentCategoryRepositoryAsync;
        }

        protected override Task<List<Recipe>> ExecuteAsync(ProcessorViewContext viewContext)
        {
            return
                _recipeToContentCategoryRepositoryAsync.Query(p => p.ContentCategoryId == viewContext.Parameters.IdCategory)
                    .Include(p => p.Recipe)
                    .Where(p => p.Recipe.StatusCode == RecordStatusCode.Active)
                    .SelectAsync(item => item.Recipe, false);
        }

        public override string ResultName => "Recipes";
    }
}