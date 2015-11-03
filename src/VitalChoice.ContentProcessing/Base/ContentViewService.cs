using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Helpers;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.ContentProcessing.Base
{
    public abstract class ContentViewService<TEntity> : IContentViewService
        where TEntity : ContentDataItem
    {
        private readonly ITtlGlobalCache _templatesCache;
        private readonly IContentProcessor<TEntity> _defaultProcessor;
        private readonly IContentProcessorService _processorService;
        protected readonly ILogger Logger;

        protected ContentViewService(
            ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessor<TEntity> defaultProcessor, IContentProcessorService processorService)
        {
            _templatesCache = templatesCache;
            _defaultProcessor = defaultProcessor;
            _processorService = processorService;
            Logger = loggerProvider.CreateLoggerDefault();
        }

        protected virtual Task<TEntity> GetData(IDictionary<string, object> queryData)
        {
            return _defaultProcessor.ExecuteAsync(queryData);
        }

        public virtual async Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData)
        {
            var contentEntity = await GetData(queryData);
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
                {_defaultProcessor.ResultName, contentEntity}
            };
            queryData.Add(_defaultProcessor.ResultName, contentEntity);

            await
                Task.WhenAll(
                    contentEntity.MasterContentItem.MasterContentItemToContentProcessors.Select(
                        p =>
                            _processorService.ExecuteAsync(
                                //ReSharper disable once AccessToModifiedClosure 
                                //We syncronize model inside service
                                p.ContentProcessor.Type, queryData, model)));
            await Task.WhenAll(contentEntity.ContentItem.ContentItemToContentProcessors.Select(
                p =>
                    _processorService.ExecuteAsync(
                        //ReSharper disable once AccessToModifiedClosure 
                        //We syncronize model inside service
                        p.ContentProcessor.Type, queryData, model)));
            
            var templatingModel = new ExpandoObject();
            model.CopyTo(templatingModel);
                       
            var generatedHtml = template.Generate(templatingModel);

            var toReturn = new ContentViewModel
            {
                Body = generatedHtml,
                Title = contentEntity.ContentItem.Title,
                MetaDescription = contentEntity.ContentItem.MetaDescription,
                MetaKeywords = contentEntity.ContentItem.MetaKeywords,
            };

            return toReturn;
        }
    }
}