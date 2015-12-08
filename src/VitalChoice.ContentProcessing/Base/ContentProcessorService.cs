﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Base
{
    public class ContentProcessorService : IContentProcessorService
    {
        private readonly IIndex<string, IContentProcessor> _processors;

        public ContentProcessorService(IIndex<string, IContentProcessor> processors)
        {
            _processors = processors;
        }

        public IContentProcessor GetContentProcessorByName(string name)
        {
            IContentProcessor processor;
            if (_processors.TryGetValue(name, out processor))
            {
                return processor;
            }
            return null;
        }

        public async Task<IDictionary<string, object>> ExecuteAsync(string processorName, ContentViewContext viewContext)
        {
            var modelContainer = new Dictionary<string, object>();
            await ExecuteAsync(processorName, viewContext, modelContainer);
            return modelContainer;
        }

        public async Task ExecuteAsync(string processorName,
            ContentViewContext viewContext, IDictionary<string, object> modelContainer)
        {
            var processor = GetContentProcessorByName(processorName);
            if (processor != null)
            {
                var processorResult = await processor.ExecuteUntypedAsync(viewContext);
                lock (modelContainer)
                {
                    if (modelContainer.ContainsKey(processor.ResultName))
                    {
                        modelContainer[processor.ResultName] = processorResult;
                    }
                    else
                    {
                        modelContainer.Add(processor.ResultName, processorResult);
                    }
                }
            }
        }
    }
}