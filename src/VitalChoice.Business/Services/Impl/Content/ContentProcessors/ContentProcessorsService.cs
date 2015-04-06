using System;
using System.Collections.Generic;
using VitalChoice.Business.Services.Contracts.Content.ContentProcessors;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Impl.Content.ContentProcessors
{
	public class ContentProcessorsService : IContentProcessorsService
	{
        private const string PROCESSORS_NAME_SPACE = "VitalChoice.Business.Services.Impl.Content.ContentProcessors.";

        private Dictionary<string, IContentProcessor> processors;

        public IContentProcessor GetContentProcessorByName(string name)
        {
            var fullName = PROCESSORS_NAME_SPACE + name;
            IContentProcessor toReturn = null;

            if (processors == null)
                processors = new Dictionary<string, IContentProcessor>();

            if (processors.ContainsKey(fullName))
                return processors[fullName];

            Type type = Type.GetType(fullName);
            processors.Add(fullName, (IContentProcessor)Activator.CreateInstance(type));

            return processors[fullName];
        }
	}
}