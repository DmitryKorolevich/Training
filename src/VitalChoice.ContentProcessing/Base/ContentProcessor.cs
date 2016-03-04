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
            public ProcessorViewContext(TModel parameters, ContentDataItem entity, ClaimsPrincipal user)
            {
                Parameters = parameters;
                Entity = entity;
                User = user;
            }

            public TModel Parameters { get; }
            public ContentDataItem Entity { get; }
            public ClaimsPrincipal User { get; }
        }

        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        protected abstract Task<TResult> ExecuteAsync(ProcessorViewContext viewContext);

        public Task<TResult> ExecuteAsync(ContentViewContext viewContext)
        {
            return
                ExecuteAsync(
                    new ProcessorViewContext((TModel) _mapper.FromDictionary(viewContext.Parameters as IDictionary<string, object>, false),
                        viewContext.BaseEntity, viewContext.User));
        }

        public async Task<object> ExecuteUntypedAsync(ContentViewContext viewContext)
        {
            return
                await
                    ExecuteAsync(
                        new ProcessorViewContext(
                            (TModel) _mapper.FromDictionary(viewContext.Parameters as IDictionary<string, object>, false),
                            viewContext.BaseEntity, viewContext.User));
        }

        public abstract string ResultName { get; }
    }

    public abstract class ContentProcessor<TResult, TModel, TEntity> : IContentProcessor<TResult>
        where TEntity : ContentDataItem
        where TModel: class, new()
    {
        protected class ProcessorViewContext
        {
            public ProcessorViewContext(TModel parameters, TEntity entity, ClaimsPrincipal user)
            {
                Parameters = parameters;
                Entity = entity;
                User = user;
            }

            public TModel Parameters { get; }
            public TEntity Entity { get; }
            public ClaimsPrincipal User { get; }
        }

        private readonly IObjectMapper<TModel> _mapper;

        protected ContentProcessor(IObjectMapper<TModel> mapper)
        {
            _mapper = mapper;
        }

        protected abstract Task<TResult> ExecuteAsync(ProcessorViewContext viewContext);

        public Task<TResult> ExecuteAsync(ContentViewContext viewContext)
        {
            return
                ExecuteAsync(
                    new ProcessorViewContext((TModel) _mapper.FromDictionary(viewContext.Parameters as IDictionary<string, object>, false),
                        (TEntity) viewContext.BaseEntity, viewContext.User));
        }

        public async Task<object> ExecuteUntypedAsync(ContentViewContext viewContext)
        {
            return
                await
                    ExecuteAsync(
                        new ProcessorViewContext(
                            (TModel) _mapper.FromDictionary(viewContext.Parameters as IDictionary<string, object>, false),
                            (TEntity) viewContext.BaseEntity, viewContext.User));
        }

        public abstract string ResultName { get; }
    }
}