using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Infrastructure.Azure
{
    public interface ITableLogsClient
    {
        PagedList<TableLogEntity> GetLogs(DateTime start, DateTime end, string appName, string logLevel = null, string message = null,
            string source = null, int page = 0, int pageSize = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT,
            SortFilter sorting = default(SortFilter));
        void WriteLogEntry(string source, string message, NLog.LogLevel logLevel);
    }
}