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
using Templates;
using System.Dynamic;
using VitalChoice.Ecommerce.Domain.Helpers;
using Templates.Runtime;

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
            if (modelType != null)
            {
                toReturn = modelType.GetProperties().Select(p => p.Name).ToList();
            }
            return toReturn;
        }

        public async Task<PagedList<EmailTemplate>> GetEmailTemplatesAsync(FilterBase filter)
        {
            var toReturn = await _emailTemplateRepository.Query(p => p.StatusCode != RecordStatusCode.Deleted).Include(p => p.ContentItem).
                Include(p => p.User).ThenInclude(p => p.Profile).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<EmailTemplate> GetEmailTemplateAsync(int id)
        {
            var toReturn = (await _emailTemplateRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).Include(p => p.ContentItem).Include(p => p.User).ThenInclude(p => p.Profile).
                SelectAsync(false)).FirstOrDefault();
            if (toReturn != null)
            {
                toReturn.ModelPropertyNames = GetEmailTemplateModelPropertyNames(toReturn.ModelType);
            }

            return toReturn;
        }

        private readonly string DefaultModelName = "Model";
        private readonly string SubjectMasterTemplate = "<%" + Environment.NewLine + "<default> -> (Model)" + Environment.NewLine + "{{ data }} :: dynamic" + Environment.NewLine + "%>";

        public async Task<BasicEmail> GenerateEmailAsync(string name, EmailTemplateDataModel model)
        {
            var result = await GenerateEmailsAsync(name, new List<EmailTemplateDataModel>() { model });
            return result.Count == 1 ? result.First().Value : null;
        }

        public async Task<Dictionary<EmailTemplateDataModel, BasicEmail>> GenerateEmailsAsync(string name, ICollection<EmailTemplateDataModel> models)
        {
            Dictionary<EmailTemplateDataModel, BasicEmail> toReturn = new Dictionary<EmailTemplateDataModel, BasicEmail>();
            var emailTemplate = (await _emailTemplateRepository.Query(p => p.Name == name && p.StatusCode != RecordStatusCode.Deleted).
                Include(p => p.ContentItem).Include(p => p.MasterContentItem).SelectAsync(false)).FirstOrDefault();
            if (emailTemplate != null && models.Count != 0)
            {
                ITtlTemplate mainTemplate;
                ITtlTemplate subjectTemplate;
                string generatedBody;
                string generatedSubject;
                try
                {
                    var templateCacheOptions = new TemplateCacheParam
                    {
                        IdMaster = emailTemplate.MasterContentItemId,
                        IdTemplate = emailTemplate.ContentItemId,
                        Master = emailTemplate.MasterContentItem.Template,
                        Template = emailTemplate.ContentItem.Template,
                        MasterUpdateDate = emailTemplate.MasterContentItem.Updated,
                        TemplateUpdateDate = emailTemplate.ContentItem.Updated,
                    };
                    mainTemplate = _templatesCache.GetOrCreateTemplate(templateCacheOptions);

                    var subjectTextTemplate = SubjectMasterTemplate.Replace("data", emailTemplate.ContentItem.Title ?? String.Empty);
                    subjectTemplate = new TtlTemplate(subjectTextTemplate, new CompileContext(models.First().GetType()));
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    return toReturn;
                }

                foreach (var model in models)
                {
                    Dictionary<string, object> templateModel = new Dictionary<string, object>
                        {
                            {DefaultModelName, model},
                        };

                    var templatingModel = new ExpandoObject();
                    templateModel.CopyToDictionary(templatingModel);

                    try
                    {
                        generatedBody = mainTemplate.Generate(templatingModel);
                        generatedSubject = subjectTemplate.Generate(templatingModel);
                        var item =new BasicEmail()
                        {
                            Subject = generatedSubject,
                            Body = generatedBody,
                        };

                        toReturn.Add(model, item);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e.Message, e);
                    }
                }
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
                dbItem.UserId = model.UserId;
                dbItem.MasterContentItemId = model.MasterContentItemId;
                dbItem.ContentItem.Updated = DateTime.Now;
                dbItem.ContentItem.Template = model.ContentItem.Template;
                dbItem.ContentItem.Title = model.ContentItem.Title;

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
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }
    }
}