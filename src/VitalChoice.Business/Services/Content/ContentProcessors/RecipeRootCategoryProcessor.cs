using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeRootCategoryProcessor : ContentProcessor<ContentCategory, ProcessorModel>
    {
        private readonly IReadRepositoryAsync<ContentCategory> _contentRepositoryAsync;

        public RecipeRootCategoryProcessor(IObjectMapper<ProcessorModel> mapper, IReadRepositoryAsync<ContentCategory> contentRepositoryAsync) : base(mapper)
        {
            _contentRepositoryAsync = contentRepositoryAsync;
        }

        public override async Task<ContentCategory> ExecuteAsync(ProcessorModel model)
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