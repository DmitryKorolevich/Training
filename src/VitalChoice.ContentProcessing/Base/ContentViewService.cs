using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.ContentProcessing.Base
{
    public abstract class ContentViewService<TEntity, TParametersModel> : IContentViewService
        where TEntity : ContentDataItem
        where TParametersModel : ContentParametersModel, new()
    {
        private readonly ITtlGlobalCache _templatesCache;
        private readonly IContentProcessorService _processorService;
        protected readonly IRepositoryAsync<TEntity> ContentRepository;
        private readonly IObjectMapper<TParametersModel> _mapper;
        private readonly IObjectMapperFactory _mapperFactory;
        private readonly AppOptions _appOptions;
        protected readonly ILogger Logger;

        protected ContentViewService(
            ITtlGlobalCache templatesCache, ILogger logger, IContentProcessorService processorService,
            IRepositoryAsync<TEntity> contentRepository, IObjectMapper<TParametersModel> mapper, IObjectMapperFactory mapperFactory,
            IOptions<AppOptions> appOptions)
        {
            _templatesCache = templatesCache;
            _processorService = processorService;
            ContentRepository = contentRepository;
            _mapper = mapper;
            Logger = logger;
            _mapperFactory = mapperFactory;
            _appOptions = appOptions.Value;
        }

        public virtual string ViewContextName => "ViewContext";
        public virtual string AppOptionsName => "AppOptions";
        public virtual string DefaultModelName => "Model";

        public async Task<ContentViewModel> GetContentAsync(ControllerContext context, ClaimsPrincipal user,
            object additionalParameters)
        {
            IDictionary<string, object> parameters = null;
            if (additionalParameters != null)
            {
                var mapper = _mapperFactory.CreateMapper(additionalParameters.GetType());
                parameters = mapper.ToDictionary(additionalParameters);
            }
            var viewContext = await GetData(await GetParameters(context, parameters), user, context);
            var contentEntity = viewContext.Entity;
            if (contentEntity == null)
            {
                return null;
            }

            ITtlTemplate template;
            try
            {
                var templateCacheOptions = new TemplateCacheParam
                {
                    IdMaster = contentEntity.MasterContentItemId,
                    IdTemplate = contentEntity.ContentItemId,
                    Master = contentEntity.MasterContentItem.Template,
                    Template = contentEntity.ContentItem.Template,
                    MasterUpdateDate = contentEntity.MasterContentItem.Updated,
                    TemplateUpdateDate = contentEntity.ContentItem.Updated,
                    ViewContext = viewContext
                };
                template = _templatesCache.GetOrCreateTemplate(templateCacheOptions);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return new ContentViewModel
                {
                    Body = (e as TemplateCompileException)?.ToString()
                };
            }

            Dictionary<string, object> model = new Dictionary<string, object>
            {
                {DefaultModelName, contentEntity},
                {ViewContextName, viewContext},
                {AppOptionsName, _appOptions},
            };
            try
            {
                foreach (var p in contentEntity.MasterContentItem.MasterContentItemToContentProcessors)
                {
                    await _processorService.ExecuteAsync(p.ContentProcessor.Type, viewContext, model);
                }
                foreach (var p in contentEntity.ContentItem.ContentItemToContentProcessors)
                {
                    await _processorService.ExecuteAsync(p.ContentProcessor.Type, viewContext, model);
                }
            }
            catch (NotFoundException e)
            {
                return null;
            }

            var templatingModel = new ExpandoObject();
            model.CopyToDictionary(templatingModel);

            var generatedHtml = template.Generate(templatingModel, callerData: viewContext);

            return CreateResult(generatedHtml, viewContext);
        }

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

        protected virtual async Task<TEntity> GetDataInternal(TParametersModel parameters, ContentViewContext viewContext)
        {
            if (!string.IsNullOrEmpty(parameters.Url))
            {
                var entity = await BuildQuery(ContentRepository.Query(FilterExpression(parameters))).SelectFirstOrDefaultAsync(false);
                return entity;
            }
            return null;
        }

        protected async Task<ContentViewContext<TEntity>> GetData(IDictionary<string, object> queryData, ClaimsPrincipal user,
            ActionContext context)
        {
            var viewContext = new ContentViewContext<TEntity>(queryData, null, user, context);
            viewContext.Entity = await GetDataInternal((TParametersModel)await _mapper.FromDictionaryAsync(queryData, false), viewContext);

            if (viewContext.Entity == null)
            {
                Logger.LogInfo(data => $"The item could not be found {{{data.FormatDictionary()}}}", queryData);
                //return explicitly null to see the real result of operation and don't look over code above regarding the real value
                return viewContext;
            }
            if (viewContext.Entity.ContentItem == null)
            {
                Logger.LogError("The item have no template.");
                return viewContext;
            }
            return viewContext;
        }

        protected virtual ContentViewModel CreateResult(string generatedHtml, ContentViewContext<TEntity> viewContext)
        {
            var entity = viewContext.Entity;
            return new ContentViewModel
            {
                Body = generatedHtml,
                Title = !String.IsNullOrEmpty(entity.ContentItem.Title) ? entity.ContentItem.Title :
                    String.Format(ContentConstants.CONTENT_PAGE_TITLE_GENERAL_FORMAT, entity.Name),
                MetaDescription = entity.ContentItem.MetaDescription,
                MetaKeywords = entity.ContentItem.MetaKeywords,
                Scripts = viewContext.Scripts,
                SocialMeta = viewContext.SocialMeta,
                CommandOptions = viewContext.CommandOptions
            };
        }

        protected virtual async Task<IDictionary<string, object>> GetParameters(ControllerContext context, IDictionary<string, object> parameters = null)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            foreach (var actionParam in context.ActionDescriptor.Parameters)
            {
                var valueProvider = await CompositeValueProvider.CreateAsync(context);
                var values = valueProvider.GetValue(actionParam.Name);
                foreach (var stringValue in values.Where(v => v != null))
                {
                    parameters.Add(actionParam.Name, stringValue);
                }
            }
            foreach (var queryParam in context.HttpContext.Request.Query)
            {
                if (!parameters.ContainsKey(queryParam.Key))
                {
                    parameters.Add(queryParam.Key, queryParam.Value.FirstOrDefault());
                }
            }
            return parameters;
        }
    }
}