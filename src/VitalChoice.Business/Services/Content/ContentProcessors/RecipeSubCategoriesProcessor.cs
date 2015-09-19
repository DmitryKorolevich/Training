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
    public class RecipeSubCategoriesProcessor : IContentProcessor
    {
        private readonly IReadRepositoryAsync<ContentCategory> _contentCategoryRepositoryAsync;

        public RecipeSubCategoriesProcessor(IReadRepositoryAsync<ContentCategory> contentCategoryRepositoryAsync)
        {
            _contentCategoryRepositoryAsync = contentCategoryRepositoryAsync;
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
                throw new Exception("No query data for RecipeSubCategoriesProcessor");
            }
            var subCategories =
                await
                    _contentCategoryRepositoryAsync.Query(
                        p => p.ParentId == categoryId && p.StatusCode == RecordStatusCode.Active)
                        .OrderBy(query => query.OrderBy(p => p.Order))
                        .SelectAsync(false);
            model.Categories = subCategories;
            return model;
        }
    }
}