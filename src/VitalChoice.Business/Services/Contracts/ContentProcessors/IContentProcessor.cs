using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Contracts.ContentProcessors
{
	public interface IContentProcessor
	{
		Task<dynamic> ExecuteAsync(dynamic model,Dictionary<string,object> queryData);
	}
}
