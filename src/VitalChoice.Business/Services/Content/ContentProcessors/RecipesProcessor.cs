using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Content.ContentProcessors;

namespace VitalChoice.Business.Services.Content.ContentProcessors
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
                    Include(p=>p.Recipe).SelectAsync(false)).Where(p=>p.Recipe.StatusCode == RecordStatusCode.Active).ToArray();
                var recipes = recipeToContentCategories.Select(item => item.Recipe).ToList();
                model.Recipes = recipes;
            }
            return model;
        }
    }
}
