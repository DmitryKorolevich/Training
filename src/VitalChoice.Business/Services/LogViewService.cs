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

        public async Task<PagedList<CommonLogItem>> GetCommonItemsAsync(string logLevel = null, string message = null, string source = null, DateTime? from = null, DateTime? to = null,
            int page = 1, int take = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT)
        {
            var query = new CommonLogQuery();
            query = query.GetItems(logLevel, message, source, from, to);

            var toRetirn = await commonLogItemsRepository.Query(query).OrderBy(x=>x.OrderByDescending(pp=>pp.Date))
                .SelectPageAsync(page, take);

            return toRetirn;
        }
    }
}
