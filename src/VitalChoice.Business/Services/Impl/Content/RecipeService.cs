using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Templates;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Base;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Business.Services.Impl.Content
{
    public class RecipeService : IRecipeService
    {
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public RecipeService(IRepositoryAsync<Recipe> recipeRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepository, IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository)
        {
            this.recipeRepository = recipeRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.recipeToContentCategoryRepository = recipeToContentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            logger = LoggerService.GetDefault();
        }
        
        public async Task<PagedList<Recipe>> GetRecipesAsync(string name = null, int? categoryId = null, int page = 1, int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT)
        {
            RecipeQuery query = new RecipeQuery();
            List<int> ids = null;
            if(categoryId.HasValue)
            {
                if (categoryId.Value != -1)
                {
                    ids = (await recipeToContentCategoryRepository.Query(p => p.ContentCategoryId == categoryId).SelectAsync(false)).Select(p => p.RecipeId).ToList();
                    query = query.WithIds(ids);
                }
                else
                {
                    ids = (await recipeToContentCategoryRepository.Query().SelectAsync(false)).Select(p => p.RecipeId).Distinct().ToList();
                    query = query.NotWithIds(ids);
                }
            }
            query=query.WithName(name).NotDeleted();
            var toReturn = await recipeRepository.Query(query).Include(p => p.RecipesToContentCategories).ThenInclude(p => p.ContentCategory).OrderBy(x => x.OrderBy(pp => pp.Name)).
                SelectPageAsync(page,take);
            return toReturn;
        }

        public async Task<Recipe> GetRecipeAsync(int id)
        {
            RecipeQuery query = new RecipeQuery().WithId(id).NotDeleted();
            var toReturn = (await recipeRepository.Query(query).Include(p=>p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                Include(p=>p.RecipesToContentCategories).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<Recipe> UpdateRecipeAsync(Recipe model)
        {
            Recipe dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new Recipe();
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.ContentItem = new ContentItem();
                dbItem.ContentItem.Created = DateTime.Now;
                dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();

                //set predefined master
                var contentType = (await contentTypeRepository.Query(p => p.Id == (int)ContentType.Recipe).SelectAsync()).FirstOrDefault();
                if (contentType == null || !contentType.DefaultMasterContentItemId.HasValue)
                {
                    throw new Exception("The default master template isn't confugurated. Please contact support.");
                }
                dbItem.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
            }
            else
            {
                dbItem = (await recipeRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                    SelectAsync()).FirstOrDefault();
                if (dbItem != null)
                {
                    foreach (var proccesorRef in dbItem.ContentItem.ContentItemToContentProcessors)
                    {
                        await contentItemToContentProcessorRepository.DeleteAsync(proccesorRef.Id);
                    }
                }
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var urlDublicatesExist = await recipeRepository.Query(p => p.Url == model.Url && p.Id != dbItem.Id).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url","Recipe with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
                dbItem.ContentItem.Updated = DateTime.Now;
                dbItem.ContentItem.Template = model.ContentItem.Template;
                dbItem.ContentItem.Description = model.ContentItem.Description;
                dbItem.ContentItem.Title = model.ContentItem.Title;
                dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;

                if (model.Id == 0)
                {
                    dbItem = await recipeRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await recipeRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> AttachRecipeToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;
            var dbItem = (await recipeRepository.Query(p => p.Id == id).Include(p=>p.RecipesToContentCategories).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var categories = (await contentCategoryRepository.Query(p => categoryIds.Contains(p.Id) && p.Type == ContentType.RecipeCategory && p.StatusCode != RecordStatusCode.Deleted).
                                 SelectAsync(false)).ToList();

                List<int> forDelete = new List<int>();
                foreach (var recipeToContentCategory in dbItem.RecipesToContentCategories)
                {
                    bool delete = true;
                    foreach (var category in categories)
                    {
                        if(recipeToContentCategory.ContentCategoryId==category.Id)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if(delete)
                    {
                        forDelete.Add(recipeToContentCategory.Id);
                    }
                }

                List<int> forAdd = new List<int>();
                foreach (var category in categories)
                {
                    bool add = true;
                    foreach (var recipeToContentCategory in dbItem.RecipesToContentCategories)
                    {
                        if (recipeToContentCategory.ContentCategoryId == category.Id)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        forAdd.Add(category.Id);
                    }
                }

                foreach (var forDeleteId in forDelete)
                {
                    await recipeToContentCategoryRepository.DeleteAsync(forDeleteId);
                }
                await recipeToContentCategoryRepository.InsertRangeAsync(forAdd.Select(p => new RecipeToContentCategory()
                {
                    ContentCategoryId = p,
                    RecipeId = dbItem.Id
                }).ToList());

                toReturn = true;
            }

            return toReturn;
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await recipeRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await recipeRepository.UpdateAsync(dbItem);

                try
                {
                    templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
                }
                catch(Exception e)
                {
                    logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }
    }
}