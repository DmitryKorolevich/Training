using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Contracts.Content.ContentProcessors
{
	public interface IContentProcessorsService
	{
        IContentProcessor GetContentProcessorByName(string name);
	}
}
