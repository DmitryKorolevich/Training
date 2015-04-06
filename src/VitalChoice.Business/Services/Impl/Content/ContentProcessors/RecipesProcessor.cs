using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Business.Services.Contracts.Content.ContentProcessors;

namespace VitalChoice.Business.Services.Impl.Content.ContentProcessors
{
    public class RecipesProcessor : IContentProcessor
    {
        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            int? categoryId = null;
            if (queryData.ContainsKey(ContentConstants.CATEGORY_ID) && queryData[ContentConstants.CATEGORY_ID] is int?)
            {
                categoryId = (int?)queryData[ContentConstants.CATEGORY_ID];
            }
            if (!categoryId.HasValue)
            {
                throw new Exception("No query data for RecipesProcessor");
            }
            using (var uof = new VitalChoiceUnitOfWork())
            {
                //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
                var repository = uof.RepositoryAsync<RecipeToContentCategory>();
                var recipeToContentCategories = (await repository.Query(p => p.ContentCategoryId== categoryId).
                    Include(p=>p.Recipe).SelectAsync(false)).ToList().Where(p=>p.Recipe.StatusCode == RecordStatusCode.Active).ToList();
                var recipes = new List<Recipe>();
                foreach(var item in recipeToContentCategories)
                {
                    recipes.Add(item.Recipe);
                }
                model.Recipes = recipes;
            }
            return model;
        }
    }
}
