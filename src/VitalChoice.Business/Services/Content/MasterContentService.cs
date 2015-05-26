using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class MasterContentService : IMasterContentService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<MasterContentItemToContentProcessor> masterContentItemToProcessorsRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IRepositoryAsync<FAQ> faqRepository;
        private readonly IRepositoryAsync<Article> articleRepository;
        private readonly IRepositoryAsync<ContentPage> contentPageRepository;
        private readonly ILogger _logger;

        public MasterContentService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<MasterContentItemToContentProcessor> masterContentItemToProcessorsRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<Recipe> recipeRepository,
            IRepositoryAsync<FAQ> faqRepository, IRepositoryAsync<Article> articleRepository, IRepositoryAsync<ContentPage> contentPageRepository)
        {
            this.masterContentItemRepository = masterContentItemRepository;
            this.masterContentItemToProcessorsRepository = masterContentItemToProcessorsRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.recipeRepository = recipeRepository;
            this.faqRepository = faqRepository;
            this.articleRepository = articleRepository;
            this.contentPageRepository = contentPageRepository;
            _logger = LoggerService.GetDefault();
        }

        public async Task<IEnumerable<ContentTypeEntity>> GetContentTypesAsync()
        {
            var toReturn = (await contentTypeRepository.Query().SelectAsync(false)).ToList();
            return toReturn;
        }

        public async Task<IEnumerable<MasterContentItem>> GetMasterContentItemsAsync(MasterContentItemListFilter filter)
        {
            var query = new MasterContentItemQuery();
            query = query.WithType(filter.Type).NotDeleted().WithStatus(filter.Status);
            var toReturn = (await masterContentItemRepository.Query(query).Include(p=>p.Type).Include(p=>p.User).ThenInclude(p => p.Profile).
                SelectAsync(false)).ToList();
            return toReturn;
        }

        public async Task<MasterContentItem> GetMasterContentItemAsync(int id)
        {
            var query = new MasterContentItemQuery();
            query = query.WithId(id).NotDeleted();
            var toReturn = (await masterContentItemRepository.Query(query).Include(p=>p.MasterContentItemToContentProcessors).Include(p=>p.Type).
                Include(p => p.User).ThenInclude(p=>p.Profile).SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem model)
        {
            MasterContentItem dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new MasterContentItem();
                dbItem.TypeId = model.Type.Id;
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.MasterContentItemToContentProcessors = new List<MasterContentItemToContentProcessor>();
                dbItem.Created = DateTime.Now;
            }
            else
            {
                dbItem = (await masterContentItemRepository.Query(p => p.Id == model.Id).Include(p=>p.MasterContentItemToContentProcessors).Include(p=>p.Type).
                    SelectAsync()).FirstOrDefault();
                if (dbItem != null)
                {
                    foreach (var proccesorRef in dbItem.MasterContentItemToContentProcessors)
                    {
                        await masterContentItemToProcessorsRepository.DeleteAsync(proccesorRef.Id);
                    }
                }
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var nameDublicatesExist = await masterContentItemRepository.Query(p => p.Name == model.Name && p.Id != dbItem.Id
                    && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                if (nameDublicatesExist)
                {
                    throw new AppValidationException("Name","Master content item with the same name is already exist.");
                }

                dbItem.Name = model.Name;
                dbItem.Template = model.Template;
                dbItem.UserId = model.UserId;
                dbItem.Updated = DateTime.Now;

                if (model.Id == 0)
                {
                    dbItem = await masterContentItemRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await masterContentItemRepository.UpdateAsync(dbItem);
                }

                foreach (var processor in model.MasterContentItemToContentProcessors)
                {
                    processor.MasterContentItemId = dbItem.Id;
                }
                await masterContentItemToProcessorsRepository.InsertRangeAsync(model.MasterContentItemToContentProcessors);

                if (model.Type.DefaultMasterContentItemId.HasValue)
                {
                    var contentType = (await contentTypeRepository.Query(p => p.Id == dbItem.TypeId).SelectAsync()).FirstOrDefault();
                    if (contentType != null && model.Type.DefaultMasterContentItemId.HasValue && 
                        contentType.DefaultMasterContentItemId != model.Type.DefaultMasterContentItemId)
                    {
                        contentType.DefaultMasterContentItemId = dbItem.Id;
                        await contentTypeRepository.UpdateAsync(contentType);
                    }
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteMasterContentItemAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await masterContentItemRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                bool relations = false;
                relations =await contentCategoryRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync();
                if (!relations)
                {
                    relations = await recipeRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync();
                }
                if (!relations)
                {
                    relations = await faqRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync();
                }
                if (!relations)
                {
                    relations = await articleRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync();
                }
                if (!relations)
                {
                    relations = await contentPageRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync();
                }

                if (relations)
                {
                    throw new AppValidationException("The given master templates is used. Reassign related entities first.");
                }
                else
                {
                    dbItem.StatusCode = RecordStatusCode.Deleted;
                    await masterContentItemRepository.UpdateAsync(dbItem);
                    toReturn = true;
                }
            }
            return toReturn;
        }
    }
}