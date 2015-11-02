using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Helpers;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentService
    {
        Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData);
    }

    public abstract class ContentService<TEntity> : IContentService
        where TEntity : ContentDataItem
    {
        private readonly ITtlGlobalCache _templatesCache;
        private readonly IContentProcessor<TEntity> _defaultProcessor;
        private readonly IContentProcessorService _processorService;
        private readonly ILogger _logger;

        protected ContentService(
            ITtlGlobalCache templatesCache, ILoggerProviderExtended loggerProvider,
            IContentProcessor<TEntity> defaultProcessor, IContentProcessorService processorService)
        {
            _templatesCache = templatesCache;
            _defaultProcessor = defaultProcessor;
            _processorService = processorService;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData)
        {
            var contentEntity = await _defaultProcessor.ExecuteAsync(queryData);
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
                _logger.LogError(e.Message, e);
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