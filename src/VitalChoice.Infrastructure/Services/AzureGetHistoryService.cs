using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Infrastructure.Services
{
    public class AzureGetHistoryService : IObjectHistoryLogService
    {
        private readonly IBlobStorageClient _blobStorageClient;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly string _objectHistoryContainerName;
        //private readonly JsonSerializer _serializer;
        private readonly Lazy<CloudTable> _table;
        private readonly ILogger _logger;

        public AzureGetHistoryService(IOptions<AppOptions> options, IBlobStorageClient blobStorageClient,
            IRepositoryAsync<AdminProfile> adminProfileRepository, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AzureGetHistoryService>();
            _objectHistoryContainerName = options.Value.AzureStorage.ObjectHistoryContainerName;
            _blobStorageClient = blobStorageClient;
            _adminProfileRepository = adminProfileRepository;
            //var settings = new JsonSerializerSettings
            //{
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    NullValueHandling = NullValueHandling.Ignore,
            //    //ContractResolver = new PublicPropertiesResolver()
            //};
            //_serializer = JsonSerializer.Create(settings);

            _table = new Lazy<CloudTable>(() =>
            {
                var storageAccount = CloudStorageAccount.Parse(options.Value.AzureStorage.StorageConnectionString);

                var tableServicePoint = ServicePointManager.FindServicePoint(storageAccount.TableEndpoint);
                tableServicePoint.ConnectionLimit = 100;

                var tableClient = storageAccount.CreateCloudTableClient();
                var result = tableClient.GetTableReference("ObjectHistory");
                result.CreateIfNotExists();
                return result;
            }, LazyThreadSafetyMode.None);
        }

        public async Task<PagedList<ObjectHistoryItem>> GetObjectHistoryLogItems(ObjectHistoryLogItemsFilter filter)
        {
            if (filter.Paging == null)
            {
                filter.Paging = new Paging
                {
                    PageIndex = 0,
                    PageItemCount = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT
                };
            }
            var query = new TableQuery<LogDataItemTableEntity>();
            var condition =
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, filter.IdObject.ToString()),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForInt("IdObjectType", QueryComparisons.Equal, (int) filter.IdObjectType));

            List<LogDataItemTableEntity> allItems = null;

            await Task.Factory.StartNew(() =>
            {
                var results =
                    _table.Value.ExecuteQuery(query.Where(condition));

                allItems = results.ToList();
            });

            if (allItems == null)
            {
                throw new InvalidOperationException();
            }

            var pagedResult = allItems.OrderByDescending(h => h.DateCreated)
                .Skip((filter.Paging.PageIndex - 1)*filter.Paging.PageItemCount)
                .Take(filter.Paging.PageItemCount).ToList();

            var adminIds = pagedResult.Where(pp => pp.IdEditedBy.HasValue).Select(pp => pp.IdEditedBy.Value).Distinct().ToList();
            var profiles = (await _adminProfileRepository.Query(p => adminIds.Contains(p.Id)).SelectAsync(false)).ToDictionary(a => a.Id);
            foreach (var item in pagedResult)
            {
                if (item.IdEditedBy.HasValue)
                {
                    item.EditedBy = profiles.GetValueOrDefault(item.IdEditedBy.Value)?.AgentId;
                }
            }

            return new PagedList<ObjectHistoryItem>(pagedResult.Select(h => new ObjectHistoryItem(h)).ToList(), allItems.Count);
        }

        public async Task<ObjectHistoryReportModel> GetObjectHistoryReport(ObjectHistoryLogItemsFilter filter)
        {
            var ids = new HashSet<int>();
            var result = new ObjectHistoryReportModel();
            var query = new TableQuery<LogDataItemTableEntity>();
            var condition =
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, filter.IdObject.ToString()),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForInt("IdObjectType", QueryComparisons.Equal, (int) filter.IdObjectType));

            LogDataItemTableEntity mainEntity = null;

            await Task.Factory.StartNew(() =>
            {
                mainEntity = _table.Value.ExecuteQuery(query.Where(condition)).OrderByDescending(h => h.DateCreated).FirstOrDefault();
            });

            if (mainEntity != null)
            {
                if (mainEntity.IdEditedBy != null)
                {
                    ids.Add(mainEntity.IdEditedBy.Value);
                }
                try
                {
                    mainEntity.Data =
                        await
                            _blobStorageClient.DownloadBlobAsStringAsync(_objectHistoryContainerName,
                                mainEntity.PartitionKey + mainEntity.RowKey);
                }
                catch (WebException e)
                {
                    _logger.LogWarning(e.ToString());
                }
            }
            result.Main = new ObjectHistoryLogListItemModel(mainEntity);
            LogDataItemTableEntity compareToEntity = null;
            if (!string.IsNullOrEmpty(filter.DataReferenceId))
            {
                var parameters = filter.DataReferenceId.Split('_');
                if (parameters.Length > 1)
                {
                    var itemCompareToCondition = TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, parameters[0]),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, parameters[1]));

                    await Task.Factory.StartNew(() =>
                    {
                        compareToEntity = _table.Value.ExecuteQuery(query.Where(itemCompareToCondition)).FirstOrDefault();
                    });

                    if (compareToEntity != null)
                    {
                        if (compareToEntity.IdEditedBy != null)
                        {
                            ids.Add(compareToEntity.IdEditedBy.Value);
                        }

                        try
                        {
                            compareToEntity.Data = await
                                _blobStorageClient.DownloadBlobAsStringAsync(_objectHistoryContainerName,
                                    parameters[0] + parameters[1]);
                        }
                        catch (WebException e)
                        {
                            _logger.LogWarning(e.ToString());
                        }
                    }

                    result.Before = new ObjectHistoryLogListItemModel(compareToEntity);
                }
            }
            var profiles = (await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false)).ToDictionary(a => a.Id);
            if (mainEntity?.IdEditedBy != null)
            {
                result.Main.EditedBy = profiles.GetValueOrDefault(mainEntity?.IdEditedBy.Value ?? 0)?.AgentId;
            }
            if (compareToEntity?.IdEditedBy != null)
            {
                result.Before.EditedBy = profiles.GetValueOrDefault(compareToEntity?.IdEditedBy.Value ?? 0)?.AgentId;
            }
            return result;
        }
    }
}