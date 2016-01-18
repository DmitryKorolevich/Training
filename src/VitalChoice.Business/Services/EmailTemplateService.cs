using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Ecommerce.Domain.Mail;
using System.Reflection;
using VitalChoice.Infrastructure.Domain.Mail;

namespace VitalChoice.Business.Services.Content
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IRepositoryAsync<EmailTemplate> _emailTemplateRepository;
        private readonly IRepositoryAsync<ContentItem> _contentItemRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> _contentTypeRepository;
        private readonly ITtlGlobalCache _templatesCache;
        private readonly ILogger _logger;

        private static List<Type> _modelTypes = new List<Type>();

        public EmailTemplateService(
            IRepositoryAsync<EmailTemplate> emailTemplateRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            ILoggerProviderExtended logger, ITtlGlobalCache templatesCache)
        {
            this._emailTemplateRepository = emailTemplateRepository;
            this._contentItemRepository = contentItemRepository;
            this._contentTypeRepository = contentTypeRepository;
            this._templatesCache = templatesCache;
            this._logger = logger.CreateLoggerDefault();
        }

        static EmailTemplateService()
        {
            //Ecommerce
            var assembly = typeof(BasicEmail).GetTypeInfo().Assembly;
            _modelTypes = assembly
                .GetExportedTypes()
                .Where(
                    t => t.GetTypeInfo().IsSubclassOf(typeof(EmailTemplateDataModel)) && !t.GetTypeInfo().IsAbstract &&
                    !t.GetTypeInfo().IsGenericType).ToList();
            //Infrastructure
            assembly = typeof(PrivacyRequestEmail).GetTypeInfo().Assembly;
            _modelTypes.AddRange(assembly
                .GetExportedTypes()
                .Where(
                    t => t.GetTypeInfo().IsSubclassOf(typeof(EmailTemplateDataModel)) && !t.GetTypeInfo().IsAbstract &&
                    !t.GetTypeInfo().IsGenericType));
        }

        private static ICollection<string> GetEmailTemplateModelPropertyNames(string typeName)
        {
            List<string> toReturn = new List<string>();
            var modelType = _modelTypes.FirstOrDefault(p => p.Name == typeName);
            if(modelType!=null)
            {
                toReturn = modelType.GetProperties(BindingFlags.Public).Select(p=>p.Name).ToList();
            }
            return toReturn;
        }

        public async Task<PagedList<EmailTemplate>> GetEmailTemplatesAsync(FilterBase filter)
        {
	        var toReturn = await _emailTemplateRepository.Query(p=>p.StatusCode!=RecordStatusCode.Deleted).Include(p=>p.ContentItem).
                Include(p => p.User).ThenInclude(p => p.Profile).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<EmailTemplate> GetEmailTemplateAsync(int id)
        {
            var toReturn = (await _emailTemplateRepository.Query(p =>p.Id==id && p.StatusCode != RecordStatusCode.Deleted).Include(p=>p.ContentItem).Include(p => p.User).ThenInclude(p => p.Profile).
                SelectAsync(false)).FirstOrDefault();
            if(toReturn!=null)
            {
                toReturn.ModelPropertyNames = GetEmailTemplateModelPropertyNames(toReturn.ModelType);
            }

            return toReturn;
        }

        public async Task<EmailTemplate> UpdateEmailTemplateAsync(EmailTemplate model)
        {
            EmailTemplate dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new EmailTemplate();
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.ContentItem = new ContentItem();
                dbItem.ContentItem.Created = DateTime.Now;

                //set predefined master
                var contentType = (await _contentTypeRepository.Query(p => p.Id == (int)ContentType.Email).SelectAsync()).FirstOrDefault();
                if (contentType == null || !contentType.DefaultMasterContentItemId.HasValue)
                {
                    throw new AppValidationException("The default master template isn't confugurated. Please contact support.");
                }
                dbItem.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
            }
            else
            {
                dbItem = (await _emailTemplateRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).
                    SelectAsync()).FirstOrDefault();
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                dbItem.Name = model.Name;
                dbItem.UserId = model.UserId;
                dbItem.ContentItem.Updated = DateTime.Now;
                dbItem.ContentItem.Template = model.ContentItem.Template;
                dbItem.ContentItem.Description = model.ContentItem.Description;
                dbItem.ContentItem.Title = model.ContentItem.Title;
                dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;

                if (model.Id == 0)
                {
                    await _emailTemplateRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    await _emailTemplateRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteEmailTemplateAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _emailTemplateRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _emailTemplateRepository.UpdateAsync(dbItem);

                try
                {
                    await _templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
                }
                catch(Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }
    }
}