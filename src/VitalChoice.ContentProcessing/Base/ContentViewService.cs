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
using Microsoft.AspNet.Mvc;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Constants;
using Microsoft.AspNet.Http.Extensions;

namespace VitalChoice.ContentProcessing.Base
{
    public class ContentRequest
    {
        public string AbsoluteUrl { get; set; }
    }

    public abstract class ContentViewService<TEntity, TParametersModel> : IContentViewService
        where TEntity : ContentDataItem
        where TParametersModel : ContentServiceModel
    {
        private readonly ITtlGlobalCache _templatesCache;
        private readonly IContentProcessorService _processorService;
        protected readonly IRepositoryAsync<TEntity> ContentRepository;
        private readonly IObjectMapper<TParametersModel> _mapper;
        private readonly IObjectMapperFactory _mapperFactory;
        protected readonly ILogger Logger;

        protected ContentViewService(
            ITtlGlobalCache templatesCache, ILogger logger, IContentProcessorService processorService,
            IRepositoryAsync<TEntity> contentRepository, IObjectMapper<TParametersModel> mapper, IObjectMapperFactory mapperFactory)
        {
            _templatesCache = templatesCache;
            _processorService = processorService;
            ContentRepository = contentRepository;
            _mapper = mapper;
            Logger = logger;
            _mapperFactory = mapperFactory;
        }

        public virtual string DefaultModelName => "Model";
        public virtual string DefaultRequestName => "Request";

        public async Task<ContentViewModel> GetContentAsync(ActionContext context, ActionBindingContext bindingContext, ClaimsPrincipal user,
            object additionalParameters)
        {
            IDictionary<string, object> parameters = null;
            if (additionalParameters != null)
            {
                var mapper = _mapperFactory.CreateMapper(additionalParameters.GetType());
                parameters = mapper.ToDictionary(additionalParameters);
            }
            var viewContext = await GetData(GetParameters(context, bindingContext, parameters), user);
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
                {DefaultModelName, contentEntity},
                {DefaultRequestName, new ContentRequest()
                    {
                        AbsoluteUrl = viewContext.Parameters.AbsoluteUrl
                    }
                },
            };
            foreach (var p in contentEntity.MasterContentItem.MasterContentItemToContentProcessors)
            {
                await _processorService.ExecuteAsync(p.ContentProcessor.Type, viewContext, model);
            }
            foreach (var p in contentEntity.ContentItem.ContentItemToContentProcessors)
            {
                await _processorService.ExecuteAsync(p.ContentProcessor.Type, viewContext, model);
            }

            var templatingModel = new ExpandoObject();
            model.CopyToDictionary(templatingModel);

            var generatedHtml = template.Generate(templatingModel);

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

        protected virtual async Task<ContentViewContext<TEntity>> GetDataInternal(TParametersModel model,
            IDictionary<string, object> parameters, ClaimsPrincipal user)
        {
            if (!string.IsNullOrEmpty(model.Url))
            {
                var entity = await BuildQuery(ContentRepository.Query(FilterExpression(model))).SelectFirstOrDefaultAsync(false);
                return new ContentViewContext<TEntity>(parameters, entity, user);
            }
            return new ContentViewContext<TEntity>(parameters, null, user);
        }

        protected async Task<ContentViewContext<TEntity>> GetData(IDictionary<string, object> queryData, ClaimsPrincipal user)
        {
            var viewContext = await GetDataInternal((TParametersModel)_mapper.FromDictionary(queryData, false), queryData, user);
            if (viewContext.Entity == null)
            {
                Logger.LogInformation("The item could not be found {" + queryData.FormatDictionary() + "}");
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
                Title = entity.ContentItem.Title,
                MetaDescription = entity.ContentItem.MetaDescription,
                MetaKeywords = entity.ContentItem.MetaKeywords,
            };
        }

        protected virtual IDictionary<string, object> GetParameters(ActionContext context, ActionBindingContext bindingContext,
            IDictionary<string, object> parameters = null)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();
            foreach (var actionParam in context.ActionDescriptor.Parameters)
            {
                var values = bindingContext.ValueProvider.GetValue(actionParam.Name).Values;
                foreach (var stringValue in values)
                {
                    parameters.Add(actionParam.Name, stringValue);
                }
            }
            foreach (var queryParam in context.HttpContext.Request.Query)
            {
                if (!parameters.ContainsKey(queryParam.Key))
                {
                    if (queryParam.Key == QueryStringConstants.PREVIEW)
                    {
                        bool result = false;
                        if (Boolean.TryParse(queryParam.Value.FirstOrDefault(), out result))
                        {
                            parameters.Add(QueryStringConstants.CPREVIEW, result);
                        }
                    }
                    else
                    {
                        parameters.Add(queryParam.Key, queryParam.Value.FirstOrDefault());
                    }
                }
            }
            var absoluteUrl = context.HttpContext.Request.GetDisplayUrl();
            parameters.Add(QueryStringConstants.ABSOLUTE_URL, absoluteUrl);
            return parameters;
        }
    }
}