using Autofac.Features.Indexed;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class ContentProcessorsService : IContentProcessorsService
    {
        private readonly IIndex<string, IContentProcessor> _processors;

        public ContentProcessorsService(IIndex<string, IContentProcessor> processors)
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
    }
}