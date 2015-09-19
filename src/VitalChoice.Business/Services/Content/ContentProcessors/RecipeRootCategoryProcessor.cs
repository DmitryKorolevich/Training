using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeRootCategoryProcessor : IContentProcessor
    {
        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            using (var uof = new VitalChoiceUnitOfWork())
            {
                //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
                var repository = uof.RepositoryAsync<ContentCategory>();
                var recipeCategories = await repository.Query(p => p.Type == ContentType.RecipeCategory && p.StatusCode == RecordStatusCode.Active).SelectAsync(false);

                ContentCategory rootCategory = recipeCategories.FirstOrDefault(p => !p.ParentId.HasValue);
                if (rootCategory == null)
                {
                    throw new Exception("No data for RecipeRootCategoryProcessor");
                }

                recipeCategories.RemoveAll(p => !p.ParentId.HasValue);
                rootCategory.CreateSubCategoriesList(recipeCategories);
                model.RootCategory = rootCategory;
            }
            return model;
        }
    }
}
