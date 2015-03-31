using System.Collections.Generic;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Contracts.ContentProcessors
{
	public interface IContentProcessorsService
	{
        IContentProcessor GetContentProcessorByName(string name);
	}
}
