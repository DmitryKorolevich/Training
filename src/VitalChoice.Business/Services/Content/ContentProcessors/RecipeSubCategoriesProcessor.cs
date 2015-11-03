﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Content.ContentProcessors
{
    public class RecipeSubCategoriesProcessor : ContentProcessor<List<ContentCategory>, RecipeParameters>
    {
        private readonly IReadRepositoryAsync<ContentCategory> _contentCategoryRepositoryAsync;

        public RecipeSubCategoriesProcessor(IObjectMapper<RecipeParameters> mapper,
            IReadRepositoryAsync<ContentCategory> contentCategoryRepositoryAsync) : base(mapper)
        {
            _contentCategoryRepositoryAsync = contentCategoryRepositoryAsync;
        }

        public override Task<List<ContentCategory>> ExecuteAsync(RecipeParameters model)
        {
            return
                _contentCategoryRepositoryAsync.Query(
                    p => p.ParentId == model.IdCategory && p.StatusCode == RecordStatusCode.Active)
                    .OrderBy(query => query.OrderBy(p => p.Order))
                    .SelectAsync(false);
        }

        public override string ResultName => "Categories";
    }
}