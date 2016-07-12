using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
using VitalChoice.Ecommerce.Domain.Helpers;

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

        public string Message1 { get; set; }

        public string Message2 { get; set; }

        public string Message3 { get; set; }

        public string Message4 { get; set; }

        public string Message5 { get; set; }

        public string Message6 { get; set; }

        public string Message7 { get; set; }

        public string Message8 { get; set; }

        public string Message9 { get; set; }

        public string Message10 { get; set; }

        public string Message11 { get; set; }

        public string Message12 { get; set; }

        public string Message13 { get; set; }

        public string Message14 { get; set; }

        public string Message15 { get; set; }

        private string _fullMessage;

        public string GetFullMessage()
        {
            if (_fullMessage != null)
                return _fullMessage;

            if (!string.IsNullOrEmpty(Message2))
            {
                var sb = new StringBuilder();
                foreach (var property in MessageProperties)
                {
                    var messagePart = property.Get(this);
                    if (!string.IsNullOrEmpty(messagePart))
                    {
                        sb.Append(messagePart);
                    }
                    else
                    {
                        break;
                    }
                }
                _fullMessage = sb.ToString();
            }
            else
            {
                _fullMessage = Message1;
            }
            return _fullMessage;
        }

        public string ShortMessage { get; set; }

        [IgnoreProperty]
        internal static List<EntityPropertyAccessors> MessageProperties { get; }

        static TableLogEntity()
        {
            MessageProperties =
                typeof(TableLogEntity).GetRuntimeProperties()
                    .Where(p => p.Name.StartsWith("Message"))
                    .OrderBy(p => int.Parse(p.Name.Substring("Message".Length)))
                    .Select(
                        p =>
                            new EntityPropertyAccessors(p.GetMethod.CompileAccessor<TableLogEntity, string>(),
                                p.SetMethod.CompileVoidAccessor<TableLogEntity, string>()))
                    .ToList();
        }
    }

    internal class EntityPropertyAccessors
    {
        public EntityPropertyAccessors(Func<TableLogEntity, string> get, Action<TableLogEntity, string> set)
        {
            Get = get;
            Set = set;
        }

        public Func<TableLogEntity, string> Get { get; }

        public Action<TableLogEntity, string> Set { get; }
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
            if (message != null)
            {
                try
                {
                    var entity = new TableLogEntity(logLevel)
                    {
                        ShortMessage = message.Substring(0, message.Length > 150 ? 150 : message.Length),
                        Source = source
                    };

                    //cut off upper maximum size
                    if (message.Length > 983040)
                    {
                        message = message.Substring(0, 983040);
                    }
                    int seed = 0;
                    int propertyIndex = 0;
                    while (message.Length - seed > 65536)
                    {
                        TableLogEntity.MessageProperties[propertyIndex].Set(entity, message.Substring(seed, 65536));
                        seed += 65536;
                        propertyIndex++;
                    }
                    if (message.Length - seed > 0)
                    {
                        TableLogEntity.MessageProperties[propertyIndex].Set(entity,
                            seed > 0 ? message.Substring(seed, message.Length - seed) : message);
                    }
                    var op = TableOperation.Insert(entity);
                    _table.Execute(op);
                }
                catch (Exception e)
                {
                    message = e.ToString();

                    var op = TableOperation.Insert(new TableLogEntity(NLog.LogLevel.Fatal)
                    {
                        ShortMessage = message.Substring(0, message.Length > 150 ? 150 : message.Length),
                        Message1 = message.Substring(0, message.Length > 65536 ? 65536 : message.Length),
                        Source = typeof(TableLogsClient).FullName
                    });
                    _table.Execute(op);
                }
            }
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
                conditions.Add(x => x.GetFullMessage().Contains(message));
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