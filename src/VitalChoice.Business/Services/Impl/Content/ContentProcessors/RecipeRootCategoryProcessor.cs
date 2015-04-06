using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Business.Services.Contracts.Content.ContentProcessors;

namespace VitalChoice.Business.Services.Impl.Content.ContentProcessors
{
    public class RecipeRootCategoryProcessor : IContentProcessor
    {
        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            using (var uof = new VitalChoiceUnitOfWork())
            {
                //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
                var repository = uof.RepositoryAsync<ContentCategory>();
                List<ContentCategory> recipeCategories = (await repository.Query(p => p.Type == ContentType.Recipe && p.StatusCode == RecordStatusCode.Active).
                    SelectAsync(false)).ToList();

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
