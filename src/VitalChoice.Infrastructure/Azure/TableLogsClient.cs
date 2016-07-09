using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using NLog;
using NLog.Common;
using NLog.Targets;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using LogLevel = Microsoft.WindowsAzure.Storage.LogLevel;
using VitalChoice.Ecommerce.Domain.Entities.Logs;

namespace VitalChoice.Infrastructure.Azure
{
    [Target("AzureTables")]
    public sealed class AzureTablesTarget : Target
    {
        public static void InitalizeTableClient(TableLogsClient client)
        {
            _tableClient = client;
        }

        private static ITableLogsClient _tableClient;

        protected override void Write(LogEventInfo logEvent)
        {
            _tableClient.WriteLogEntry(logEvent.LoggerName, logEvent.Message, logEvent.Level);
        }
    }

    public class TableLogEntity : TableEntity
    {
        public TableLogEntity(NLog.LogLevel logLevel)
        {
            PartitionKey = logLevel.ToString();
            RowKey = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
        }

        public TableLogEntity()
        {

        }

        public string Source { get; set; }

        public string Message { get; set; }

        public string ShortMessage { get; set; }
    }

    public class TableLogsClient : ITableLogsClient
    {
        private readonly CloudTable _table;
        private readonly CloudTableClient _tableClient;

        public TableLogsClient(IOptions<AppOptions> appOptions, IHostingEnvironment env)
        {
            var storageAccount = CloudStorageAccount.Parse(appOptions.Value.AzureStorage.StorageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient();
            var tableName = (env.ApplicationName + "Logs").Replace(".", "");
            _table = _tableClient.GetTableReference(tableName);
            _table.CreateIfNotExists();
        }

        public void WriteLogEntry(string source, string message, NLog.LogLevel logLevel)
        {
            var op = TableOperation.Insert(new TableLogEntity(logLevel)
            {
                Message = message,
                ShortMessage = message.Substring(0, message.Length > 150 ? 150 : message.Length),
                Source = source,
            });
            _table.Execute(op);
        }

        public PagedList<TableLogEntity> GetLogs(DateTime start, DateTime end, string appName, string logLevel = null, string message = null,
            string source = null, int page = 0, int pageSize = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT,
            SortFilter sorting = default(SortFilter))
        {
            if (page > 0)
            {
                page = page - 1;
            }
            var startCondition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual,
                start.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            var endCondition = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual,
                end.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            var resultedCondition = TableQuery.CombineFilters(startCondition, TableOperators.And, endCondition);
            if (!string.IsNullOrEmpty(logLevel))
            {
                resultedCondition = TableQuery.CombineFilters(resultedCondition, TableOperators.And,
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, logLevel));
            }
            else
            {
                string levelsCondition = null;
                foreach (var item in NLog.LogLevel.AllLevels)
                {
                    var levelCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, item.ToString());
                    levelsCondition = levelsCondition == null
                        ? levelCondition
                        : TableQuery.CombineFilters(levelsCondition, TableOperators.Or, levelCondition);
                }
                resultedCondition = TableQuery.CombineFilters(resultedCondition, TableOperators.And, levelsCondition);
            }

            var query = new TableQuery<TableLogEntity>().Where(resultedCondition);

            Func<IEnumerable<TableLogEntity>, IOrderedEnumerable<TableLogEntity>> sortable = x => x.OrderByDescending(pp => pp.RowKey);
            if (sorting != null && sorting.SortOrder != FilterSortOrder.None)
            {
                var sortOrder = sorting.SortOrder;
                switch (sorting.Path)
                {
                    case CommonLogItemSortPath.Date:
                        sortable =
                            (x) =>
                                sortOrder == FilterSortOrder.Asc
                                    ? x.OrderBy(y => y.RowKey)
                                    : x.OrderByDescending(y => y.RowKey);
                        break;
                    case CommonLogItemSortPath.LogLevel:
                        sortable =
                            (x) =>
                                sortOrder == FilterSortOrder.Asc
                                    ? x.OrderBy(y => y.PartitionKey)
                                    : x.OrderByDescending(y => y.PartitionKey);
                        break;
                    case CommonLogItemSortPath.Source:
                        sortable =
                            (x) =>
                                sortOrder == FilterSortOrder.Asc
                                    ? x.OrderBy(y => y.Source)
                                    : x.OrderByDescending(y => y.Source);
                        break;
                    case CommonLogItemSortPath.ShortMessage:
                        sortable =
                            (x) =>
                                sortOrder == FilterSortOrder.Asc
                                    ? x.OrderBy(y => y.ShortMessage)
                                    : x.OrderByDescending(y => y.ShortMessage);
                        break;
                }
            }

            List<Func<TableLogEntity, bool>> conditions = new List<Func<TableLogEntity, bool>>();

            if (!string.IsNullOrWhiteSpace(message))
            {
                conditions.Add(x => x.Message.Contains(message));
            }
            if (!string.IsNullOrWhiteSpace(source))
            {
                conditions.Add(x => x.Source.Contains(source));
            }

            var tableName = (appName + "Logs").Replace(".", "");
            var table = _tableClient.GetTableReference(tableName);
            table.CreateIfNotExists();
            var items = table.ExecuteQuery(query);

            foreach (var condition in conditions)
            {
                items = items.Where(condition);
            }

            List<TableLogEntity> results;

            if (page == 0)
            {
                results = new List<TableLogEntity>(sortable(items).Take(pageSize));
            }
            else
            {
                results = new List<TableLogEntity>(sortable(items).Skip(page*pageSize).Take(pageSize));
            }

            int totalCount;

            if (results.Count < pageSize)
            {
                totalCount = page*pageSize + results.Count;
            }
            else
            {
                totalCount = (page + 2)*pageSize;
            }

            return new PagedList<TableLogEntity>(results, totalCount);
        }
    }
}