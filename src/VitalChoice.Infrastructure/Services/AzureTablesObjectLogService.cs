using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Infrastructure.Services
{
    public class AzureTablesObjectLogService : IObjectLogItemExternalService
    {
        private readonly IBlobStorageClient _blobStorageClient;
        private readonly string _objectHistoryContainerName;
        private readonly JsonSerializer _serializer;
        private readonly CloudTable _table;

        public AzureTablesObjectLogService(IOptions<AppOptions> options, IBlobStorageClient blobStorageClient)
        {
            _objectHistoryContainerName = options.Value.AzureStorage.ObjectHistoryContainerName;
            _blobStorageClient = blobStorageClient;
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                //ContractResolver = new PublicPropertiesResolver()
            };
            _serializer = JsonSerializer.Create(settings);

            var storageAccount = CloudStorageAccount.Parse(options.Value.AzureStorage.StorageConnectionString);

            var tableServicePoint = ServicePointManager.FindServicePoint(storageAccount.TableEndpoint);
            tableServicePoint.ConnectionLimit = 100;

            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("ObjectHistory");
            _table.CreateIfNotExists();
        }

        private enum LogObjectType
        {
            Dynamic,
            ContentDataItem,
            LogEntity,
            Entity,
            Other
        }

        public async Task LogItems<T>(IEnumerable<T> models, bool logFullObjects = true)
            where T : class
        {
            if (models != null)
            {
                LogObjectType logType;
                var type = typeof(T);
                if (type.GetTypeInfo().IsSubclassOf(typeof(MappedObject)))
                {
                    logType = LogObjectType.Dynamic;
                }
                else if (type.GetTypeInfo().IsSubclassOf(typeof(ContentDataItem)))
                {
                    logType = LogObjectType.ContentDataItem;
                }
                else if (type.GetTypeInfo().IsSubclassOf(typeof(LogEntity)))
                {
                    logType = LogObjectType.LogEntity;
                }
                else if (type.GetTypeInfo().IsSubclassOf(typeof(Entity)))
                {
                    logType = LogObjectType.Entity;
                }
                else
                {
                    logType = LogObjectType.Other;
                }
                var objectType = GetObjectType(type.Name, logType);

                IEnumerable<LogDataItemTableEntity> resultItems;
                switch (logType)
                {
                    case LogObjectType.Dynamic:
                        resultItems = models.Select(model =>
                        {
                            var item = TransformForDynamic(model as MappedObject, objectType);
                            AddFullData<T>(logFullObjects, item, model);
                            return item;
                        });
                        break;
                    case LogObjectType.ContentDataItem:
                        resultItems = models.Select(model =>
                        {
                            var item = TransformForContentDataItem(model as ContentDataItem, objectType);
                            AddFullData(logFullObjects, item, model);
                            return item;
                        });
                        break;
                    case LogObjectType.LogEntity:
                        resultItems = models.Select(model =>
                        {
                            var item = TransformForLogEntity(model as LogEntity, objectType);
                            AddFullData(logFullObjects, item, model);
                            return item;
                        });
                        break;
                    case LogObjectType.Entity:
                        resultItems = models.Select(model =>
                        {
                            var item = TransformForEntity(model as Entity, objectType);
                            AddFullData(logFullObjects, item, model);
                            return item;
                        });
                        break;
                    case LogObjectType.Other:
                        resultItems = models.Select(model =>
                        {
                            var item = TransformForOther(model, objectType);
                            AddFullData(logFullObjects, item, model);
                            return item;
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                List<Task> uploadTasks = new List<Task>();
                foreach (var item in resultItems)
                {
                    if (!string.IsNullOrEmpty(item.Data))
                    {
                        var blobUploadTask =
                            Task.Factory.StartNew(
                                () =>
                                    _blobStorageClient.UploadBlobAsStringAsync(_objectHistoryContainerName,
                                        item.PartitionKey + item.RowKey, item.Data).GetAwaiter().GetResult());
                        uploadTasks.Add(blobUploadTask);
                    }

                    var tableInsertTask = Task.Factory.StartNew(() =>
                    {
                        var op = TableOperation.Insert(item);
                        _table.Execute(op);
                    });
                    uploadTasks.Add(tableInsertTask);
                }
                await Task.WhenAll(uploadTasks).ConfigureAwait(false);
            }
        }

        private void AddFullData<T>(bool logFullObjects, LogDataItemTableEntity item, T model) where T : class
        {
            if (!logFullObjects)
                return;

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new JsonTextWriter(stringWriter))
                {
                    _serializer.Serialize(writer, model);
                }
                item.Data = stringWriter.ToString();
            }
        }

        public Task LogItems<T>(bool logFullObjects, params T[] models) where T : class
        {
            return LogItems(models, logFullObjects);
        }

        public Task LogItem<T>(T model, bool logFullObjects = true) where T : class
        {
            return LogItems(logFullObjects, model);
        }

        private LogDataItemTableEntity TransformForDynamic(MappedObject model, ObjectType objectType)
        {
            LogDataItemTableEntity item = new LogDataItemTableEntity((int) objectType, model.Id)
            {
                IdObjectStatus = model.StatusCode,
                IdEditedBy = model.IdEditedBy
            };
            if (objectType == ObjectType.Order)
            {
                var order = model as OrderDynamic;
                if (order != null)
                {
                    item.OptionalData = $"All:{(int?) order.OrderStatus},P:{(int?) order.POrderStatus},NP:{(int?) order.NPOrderStatus}";
                }
                var refund = model as OrderRefundDynamic;
                if (refund != null)
                {
                    item.OptionalData = $"All:{(int?) refund.OrderStatus}";
                }
            }
            return item;
        }

        private LogDataItemTableEntity TransformForContentDataItem(ContentDataItem model, ObjectType objectType)
        {
            LogDataItemTableEntity item = new LogDataItemTableEntity((int) objectType, model.Id)
            {
                IdObjectStatus = (int) model.StatusCode,
                IdEditedBy = model.UserId
            };
            return item;
        }

        private LogDataItemTableEntity TransformForLogEntity(LogEntity model, ObjectType objectType)
        {
            LogDataItemTableEntity item = new LogDataItemTableEntity((int) objectType, model.Id)
            {
                IdObjectStatus = (int) model.StatusCode,
                IdEditedBy = model.IdEditedBy
            };
            return item;
        }

        private LogDataItemTableEntity TransformForEntity(Entity model, ObjectType objectType)
        {
            LogDataItemTableEntity item = new LogDataItemTableEntity((int) objectType, model.Id);
            return item;
        }

        private LogDataItemTableEntity TransformForOther(object model, ObjectType objectType)
        {
            LogDataItemTableEntity item = new LogDataItemTableEntity((int) objectType, Guid.NewGuid().ToString("N"));
            return item;
        }

        private ObjectType GetObjectType(string name, LogObjectType logType)
        {
            ObjectType result = ObjectType.Unknown;
            if (logType != LogObjectType.Other)
            {
                if (logType == LogObjectType.Dynamic)
                {
                    name = name.Replace("Refund", "");
                    name = name.Replace("Dynamic", "");
                }
                Enum.TryParse<ObjectType>(name, out result);
                if (result == default(ObjectType))
                {
                    result = ObjectType.Unknown;
                }
            }
            return result;
        }
    }
}