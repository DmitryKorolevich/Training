using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.ContentProcessing.Base
{
    public abstract class ContentProcessor<TResult, TModel> : IContentProcessor<TResult>
        where TModel: class, new()
    {
        protected class ProcessorViewContext
        {
            public ProcessorViewContext(TModel parameters, ContentDataItem entity, ClaimsPrincipal user,
                ViewContentCommandOptions commandOptions)
            {
                Parameters = parameters;
                Entity = entity;
                User = user;
                CommandOptions = commandOptions;
            }

            public TModel Parameters { get; }
            public ContentDataItem Entity { get; }
            public ClaimsPrincipal User { get; }
            public ViewContentCommandOptions CommandOptions { get; set; }
        }

        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        protected abstract Task<TResult> ExecuteAsync(ProcessorViewContext viewContext);

        public async Task<TResult> ExecuteAsync(ContentViewContext viewContext)
        {
            return await ExecuteAsync(
                    new ProcessorViewContext((TModel)await _mapper.FromDictionaryAsync(viewContext.Parameters as IDictionary<string, object>, false),
                        viewContext.BaseEntity, viewContext.User, viewContext.CommandOptions));
        }

        public async Task<object> ExecuteUntypedAsync(ContentViewContext viewContext)
        {
            return
                await
                    ExecuteAsync(
                        new ProcessorViewContext(
                            (TModel) await _mapper.FromDictionaryAsync(viewContext.Parameters as IDictionary<string, object>, false),
                            viewContext.BaseEntity, viewContext.User, viewContext.CommandOptions));
        }

        public abstract string ResultName { get; }
    }

    public abstract class ContentProcessor<TResult, TModel, TEntity> : IContentProcessor<TResult>
        where TEntity : ContentDataItem
        where TModel: class, new()
    {
        protected class ProcessorViewContext
        {
            public ProcessorViewContext(TModel parameters, TEntity entity, ClaimsPrincipal user,
                ViewContentCommandOptions commandOptions)
            {
                Parameters = parameters;
                Entity = entity;
                User = user;
                CommandOptions = commandOptions;
            }

            public TModel Parameters { get; }
            public TEntity Entity { get; }
            public ClaimsPrincipal User { get; }
            public ViewContentCommandOptions CommandOptions { get; set; }
        }

        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        protected abstract Task<TResult> ExecuteAsync(ProcessorViewContext viewContext);

        public async Task<TResult> ExecuteAsync(ContentViewContext viewContext)
        {
            return
                await ExecuteAsync(
                    new ProcessorViewContext((TModel)await _mapper.FromDictionaryAsync(viewContext.Parameters as IDictionary<string, object>, false),
                        (TEntity) viewContext.BaseEntity, viewContext.User, viewContext.CommandOptions));
        }

        public async Task<object> ExecuteUntypedAsync(ContentViewContext viewContext)
        {
            return
                await
                    ExecuteAsync(
                        new ProcessorViewContext(
                            (TModel)await _mapper.FromDictionaryAsync(viewContext.Parameters as IDictionary<string, object>, false),
                            (TEntity) viewContext.BaseEntity, viewContext.User, viewContext.CommandOptions));
        }

        public abstract string ResultName { get; }
    }
}