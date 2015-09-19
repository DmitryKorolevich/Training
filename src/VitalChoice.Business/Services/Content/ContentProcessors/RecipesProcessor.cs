using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipesProcessor : IContentProcessor
    {
        private readonly IReadRepositoryAsync<RecipeToContentCategory> _recipeToContentCategoryRepositoryAsync;

        public RecipesProcessor(IReadRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepositoryAsync)
        {
            _recipeToContentCategoryRepositoryAsync = recipeToContentCategoryRepositoryAsync;
        }


        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            int? categoryId = null;
            if (queryData.ContainsKey(ContentConstants.CATEGORY_ID) && queryData[ContentConstants.CATEGORY_ID] is int?)
            {
                categoryId = (int?) queryData[ContentConstants.CATEGORY_ID];
            }
            if (!categoryId.HasValue)
            {
                throw new Exception("No query data for RecipesProcessor");
            }
            var recipes = await _recipeToContentCategoryRepositoryAsync.Query(p => p.ContentCategoryId == categoryId).
                Include(p => p.Recipe)
                .Where(p => p.Recipe.StatusCode == RecordStatusCode.Active)
                .SelectAsync(item => item.Recipe, false);
            model.Recipes = recipes;
            return model;
        }
    }
}