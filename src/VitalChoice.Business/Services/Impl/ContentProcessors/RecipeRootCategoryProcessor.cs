using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Contracts.ContentProcessors;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.UnitOfWork;

namespace VitalChoice.Business.Services.Impl.ContentProcessors
{
    public class RecipeRootCategoryProcessor : IContentProcessor
    {
        public async Task<dynamic> ExecuteAsync(dynamic model, Dictionary<string, object> queryData)
        {
            using (var uof = new VitalChoiceUnitOfWork())
            {
                //TODO: - use standard where syntax instead of this logic(https://github.com/aspnet/EntityFramework/issues/1460)
                var masterTemplateIds = (await uof.RepositoryAsync<MasterContentItem>().Query(p => p.Type == ContentType.Recipe).SelectAsync(false)).ToList().Select(p => p.Id);
                var repository = uof.RepositoryAsync<ContentCategory>();
                List<ContentCategory> recipeCategories = (await repository.Query(p => p.StatusCode == RecordStatusCode.Active &&
                    masterTemplateIds.Contains(p.MasterContentItemId)).
                    SelectAsync(false)).ToList();

                ContentCategory rootCategory = recipeCategories.FirstOrDefault(p => !p.ParentId.HasValue);
                if (rootCategory == null)
                {
                    throw new Exception("No data for RecipeRootCategoryProcessor");
                }

                recipeCategories.RemoveAll(p => !p.ParentId.HasValue);
                var allowToFindSubCategories = true;
                while (allowToFindSubCategories)
                {
                    allowToFindSubCategories = CreateSubCategoriesList(rootCategory, recipeCategories);
                }
                model.RootCategory = rootCategory;
            }
            return model;
        }

        public bool CreateSubCategoriesList(ContentCategory root, IEnumerable<ContentCategory> heapCategories)
        {
            //categories added
            bool toReturn = false;
            if (root.SubCategories == null)
            {
                var subCategories = heapCategories.Where(p => p.ParentId == root.Id);
                heapCategories.ToList().RemoveAll(p => p.ParentId == root.Id);
                foreach (var subCategory in subCategories)
                {
                    subCategory.Parent = root;
                }
                root.SubCategories = subCategories.ToList();
                toReturn = root.SubCategories.Count() > 0;
            }
            else
            {
                foreach (var subCategory in root.SubCategories)
                {
                    toReturn = toReturn || CreateSubCategoriesList(subCategory, heapCategories);
                }
            }
            return toReturn;
        }
    }
}
