using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeRootModel
    {

    }

    public class RecipeRootCategoryProcessor : ContentProcessor<ContentCategory, RecipeRootModel>
    {
        private readonly IReadRepositoryAsync<ContentCategory> _contentRepositoryAsync;

        public RecipeRootCategoryProcessor(IObjectMapper<RecipeRootModel> mapper,
            IReadRepositoryAsync<ContentCategory> contentRepositoryAsync) : base(mapper)
        {
            _contentRepositoryAsync = contentRepositoryAsync;
        }

        protected override async Task<ContentCategory> ExecuteAsync(ProcessorViewContext viewContext)
        {
            var recipeCategories =
                await
                    _contentRepositoryAsync.Query(
                        p =>
                            p.Type == ContentType.RecipeCategory && p.StatusCode == RecordStatusCode.Active &&
                            !p.ParentId.HasValue)
                        .SelectAsync(false);

            ContentCategory rootCategory = recipeCategories.FirstOrDefault(p => !p.ParentId.HasValue);
            if (rootCategory == null)
            {
                throw new Exception("No data for RecipeRootCategoryProcessor");
            }

            recipeCategories.RemoveAll(p => !p.ParentId.HasValue);
            rootCategory.CreateSubCategoriesList(recipeCategories);
            return rootCategory;
        }

        public override string ResultName => "RootCategory";
    }
}