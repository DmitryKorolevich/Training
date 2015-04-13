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
    public class RecipeSubCategoriesProcessor : IContentProcessor
    {
        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            int? categoryId = null;
            if(queryData.ContainsKey(ContentConstants.CATEGORY_ID) && queryData[ContentConstants.CATEGORY_ID] is int?)
            {
                categoryId = (int?)queryData[ContentConstants.CATEGORY_ID];
            }
            if(!categoryId.HasValue)
            { 
                throw new Exception("No query data for RecipeSubCategoriesProcessor");
            }
            using (var uof = new VitalChoiceUnitOfWork())
            {
                var repository = uof.RepositoryAsync<ContentCategory>();
                var subCategories = (await repository.Query(p => p.ParentId== categoryId && p.StatusCode == RecordStatusCode.Active).
                    SelectAsync(false)).ToList();
                model.Categories = subCategories.OrderBy(p => p.Order).ToList();
            }
            return model;
        }
    }
}