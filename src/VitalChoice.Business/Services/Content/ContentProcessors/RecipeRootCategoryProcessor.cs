using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeRootCategoryProcessor : IContentProcessor
    {
        private readonly IReadRepositoryAsync<ContentCategory> _contentRepositoryAsync;

        public RecipeRootCategoryProcessor(IReadRepositoryAsync<ContentCategory> contentRepositoryAsync)
        {
            _contentRepositoryAsync = contentRepositoryAsync;
        }

        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            var recipeCategories =
                await
                    _contentRepositoryAsync.Query(
                        p => p.Type == ContentType.RecipeCategory && p.StatusCode == RecordStatusCode.Active && !p.ParentId.HasValue)
                        .SelectAsync(false);

            ContentCategory rootCategory = recipeCategories.FirstOrDefault(p => !p.ParentId.HasValue);
            if (rootCategory == null)
            {
                throw new Exception("No data for RecipeRootCategoryProcessor");
            }

            recipeCategories.RemoveAll(p => !p.ParentId.HasValue);
            rootCategory.CreateSubCategoriesList(recipeCategories);
            model.RootCategory = rootCategory;
            return model;
        }
    }
}