using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeParameters : ProcessorModel
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

        public override Task<List<Recipe>> ExecuteAsync(RecipeParameters model)
        {
            return
                _recipeToContentCategoryRepositoryAsync.Query(p => p.ContentCategoryId == model.IdCategory)
                    .Include(p => p.Recipe)
                    .Where(p => p.Recipe.StatusCode == RecordStatusCode.Active)
                    .SelectAsync(item => item.Recipe, false);
        }

        public override string ResultName => "Recipes";
    }
}