using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Logs;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
	public interface ILogViewService
	{
	    Task<PagedList<CommonLogItem>> GetCommonItemsAsync(string logLevel = null, string message = null,
	        string source = null, DateTime? @from = null, DateTime? to = null, int page = 1,
	        int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT, SortFilter sorting = default(SortFilter));
	}
}
