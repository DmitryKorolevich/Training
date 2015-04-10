using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using Microsoft.Framework.ConfigurationModel;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Business.Services.Contracts.Content;

namespace VitalChoice.Business.Services.Impl.Content
{
    public class MasterContentService : IMasterContentService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<MasterContentItemToContentProcessor> masterContentItemToProcessorsRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly ILogger _logger;

        public MasterContentService(IRepositoryAsync<MasterContentItem> masterContentItemRepository, IRepositoryAsync<MasterContentItemToContentProcessor> masterContentItemToProcessorsRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<Recipe> recipeRepository)
        {
            this.masterContentItemRepository = masterContentItemRepository;
            this.masterContentItemToProcessorsRepository = masterContentItemToProcessorsRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.recipeRepository = recipeRepository;
            _logger = LoggerService.GetDefault();
        }

        public async Task<IEnumerable<ContentTypeEntity>> GetContentTypesAsync()
        {
            var toReturn = (await contentTypeRepository.Query().SelectAsync(false)).ToList();
            return toReturn;
        }

        public async Task<IEnumerable<MasterContentItem>> GetMasterContentItemsAsync(ContentType? type = null, RecordStatusCode? status = null)
        {
            var query = new MasterContentItemQuery();
            query = query.WithType(type).NotDeleted().WithStatus(status);
            var toReturn = (await masterContentItemRepository.Query(query).SelectAsync(false)).ToList();
            return toReturn;
        }

        public async Task<MasterContentItem> GetMasterContentItemAsync(int id)
        {
            var query = new MasterContentItemQuery();
            query = query.WithId(id).NotDeleted();
            var toReturn = (await masterContentItemRepository.Query(query).Include(p=>p.MasterContentItemToContentProcessors).Include(p=>p.Type).SelectAsync(false)).FirstOrDefault();
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
                var nameDublicatesExist = await masterContentItemRepository.Query(p => p.Name == model.Name && p.Id != dbItem.Id).SelectAnyAsync();
                if (nameDublicatesExist)
                {
                    throw new AppValidationException("Name","Master content item with the same name is already exist.");
                }

                dbItem.Name = model.Name;
                dbItem.Template = model.Template;

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
