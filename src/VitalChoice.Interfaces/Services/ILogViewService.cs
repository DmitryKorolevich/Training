using System;
using System.Threading.Tasks;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Interfaces.Services
{
	public interface ILogViewService
	{
	    Task<PagedList<CommonLogItem>> GetCommonItemsAsync(string logLevel = null, string message = null,
	        string source = null, DateTime? @from = null, DateTime? to = null, int page = 1,
	        int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT, SortFilter sorting = null);
	}
}
