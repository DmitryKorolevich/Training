using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Interfaces.Services.Content.ContentProcessors
{
	public interface IContentProcessor
	{
		Task<dynamic> ExecuteAsync(dynamic model,Dictionary<string,object> queryData);
	}
}
