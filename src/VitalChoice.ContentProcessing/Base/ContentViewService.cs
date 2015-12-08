using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Interfaces.Services;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Base
{
    public abstract class ContentViewService<TEntity, TParametersModel> : IContentViewService
        where TEntity : ContentDataItem
        where TParametersModel : ContentServiceModel
    {
        private readonly ITtlGlobalCache _templatesCache;
        private readonly IContentProcessorService _processorService;
        protected readonly IRepositoryAsync<TEntity> ContentRepository;
        private readonly IObjectMapper<TParametersModel> _mapper;
        protected readonly ILogger Logger;

        protected ContentViewService(
            ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider, IContentProcessorService processorService,
            IRepositoryAsync<TEntity> contentRepository, IObjectMapper<TParametersModel> mapper)
        {
            _templatesCache = templatesCache;
            _processorService = processorService;
            ContentRepository = contentRepository;
            _mapper = mapper;
            Logger = loggerProvider.CreateLoggerDefault();
        }

        public virtual string DefaultModelName => "Model";

        protected virtual Expression<Func<TEntity, bool>> FilterExpression(TParametersModel model) =>
            p => p.Url == model.Url && p.StatusCode != RecordStatusCode.Deleted;

        protected virtual IQueryFluent<TEntity> BuildQuery(IQueryFluent<TEntity> query)
        {
            return query.Include(p => p.MasterContentItem)
                .ThenInclude(p => p.MasterContentItemToContentProcessors)
                .ThenInclude(p => p.ContentProcessor)
                .Include(p => p.ContentItem)
                .ThenInclude(p => p.ContentItemToContentProcessors)
                .ThenInclude(p => p.ContentProcessor);
        }

        protected virtual async Task<ContentViewContext<TEntity>> GetDataInternal(TParametersModel model,
            IDictionary<string, object> parameters)
        {
            if (!string.IsNullOrEmpty(model.Url))
            {
                var entity = await BuildQuery(ContentRepository.Query(FilterExpression(model))).SelectFirstOrDefaultAsync(false);
                return new ContentViewContext<TEntity>(parameters, entity);
            }
            return new ContentViewContext<TEntity>(parameters, null);
        }

        protected async Task<ContentViewContext<TEntity>> GetData(IDictionary<string, object> queryData)
        {
            var viewContext = await GetDataInternal((TParametersModel) _mapper.FromDictionary(queryData, false), queryData);
            if (viewContext.Entity == null)
            {
                Logger.LogInformation("The item could not be found {" + queryData.FormatDictionary() + "}");
                //return explicitly null to see the real result of operation and don't look over code above regarding the real value
                return null;
            }
            if (viewContext.Entity.ContentItem == null)
            {
                Logger.LogError("The item have no template.");
                return null;
            }
            return viewContext;
        }

        protected virtual ContentViewModel CreateResult(string generatedHtml, ContentViewContext<TEntity> viewContext)
        {
            var entity = viewContext.Entity;
            return new ContentViewModel
            {
                Body = generatedHtml,
                Title = entity.ContentItem.Title,
                MetaDescription = entity.ContentItem.MetaDescription,
                MetaKeywords = entity.ContentItem.MetaKeywords,
            };
        }

        public async Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData)
        {
            var viewContext = await GetData(queryData);
            var contentEntity = viewContext.Entity;
            ITtlTemplate template;
            try
            {
                template = _templatesCache.GetOrCreateTemplate(contentEntity.MasterContentItem.Template,
                    contentEntity.ContentItem.Template, contentEntity.ContentItem.Updated,
                    contentEntity.MasterContentItem.Updated,
                    contentEntity.MasterContentItemId, contentEntity.ContentItem.Id);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
                return new ContentViewModel
                {
                    Body = (e as TemplateCompileException)?.ToString()
                };
            }

            Dictionary<string, object> model = new Dictionary<string, object>
            {
                {DefaultModelName, contentEntity}
            };

            await
                Task.WhenAll(
                    contentEntity.MasterContentItem.MasterContentItemToContentProcessors.Select(
                        p =>
                            _processorService.ExecuteAsync(
                                //ReSharper disable once AccessToModifiedClosure 
                                //We syncronize model inside service
                                p.ContentProcessor.Type, viewContext, model)));
            await Task.WhenAll(contentEntity.ContentItem.ContentItemToContentProcessors.Select(
                p =>
                    _processorService.ExecuteAsync(
                        //ReSharper disable once AccessToModifiedClosure 
                        //We syncronize model inside service
                        p.ContentProcessor.Type, viewContext, model)));

            var templatingModel = new ExpandoObject();
            model.CopyToDictionary(templatingModel);

            var generatedHtml = template.Generate(templatingModel);

            return CreateResult(generatedHtml, viewContext);
        }
    }
}