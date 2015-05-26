using System;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Log;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class LogViewService : ILogViewService
    {
        private readonly ILogsRepositoryAsync<CommonLogItem> commonLogItemsRepository;

        public LogViewService(ILogsRepositoryAsync<CommonLogItem> commonLogItemsRepository)
        {
            this.commonLogItemsRepository = commonLogItemsRepository;
        }

        public async Task<PagedList<CommonLogItem>> GetCommonItemsAsync(string logLevel = null, string message = null, string source = null, DateTime? @from = null, DateTime? to = null, int page = 1, int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT, SortFilter sorting = null)
        {
            var query = new CommonLogQuery();
            query = query.GetItems(logLevel, message, source, from, to);

			Func<IQueryable<CommonLogItem>, IOrderedQueryable<CommonLogItem>> sortable = x => x.OrderByDescending(pp => pp.Date);
			if (sorting != null)
	        {
				var sortOrder = sorting.SortOrder;
				switch (sorting.Path)
				{
					case CommonLogItemSortPath.Date:
						sortable =
							(x) =>
								sortOrder == SortOrder.Asc
									? x.OrderBy(y => y.Date)
									: x.OrderByDescending(y => y.Date);
						break;
					case CommonLogItemSortPath.LogLevel:
						sortable =
							(x) =>
								sortOrder == SortOrder.Asc
									? x.OrderBy(y => y.LogLevel)
									: x.OrderByDescending(y => y.LogLevel);
						break;
					case CommonLogItemSortPath.Source:
						sortable =
							(x) =>
								sortOrder == SortOrder.Asc
									? x.OrderBy(y => y.Source)
									: x.OrderByDescending(y => y.Source);
						break;
					case CommonLogItemSortPath.ShortMessage:
						sortable =
							(x) =>
								sortOrder == SortOrder.Asc
									? x.OrderBy(y => y.ShortMessage)
									: x.OrderByDescending(y => y.ShortMessage);
						break;
				}
			}

			var toReturn = await commonLogItemsRepository.Query(query).OrderBy(sortable)
                .SelectPageAsync(page, take);

            return toReturn;
        }
    }
}
