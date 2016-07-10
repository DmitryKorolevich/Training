using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Services
{
    public class ObjectLogItemExternalService : IObjectLogItemExternalService
    {
        private readonly IEcommerceRepositoryAsync<ObjectHistoryLogItem> _objectHistoryLogItemRepository;

        public ObjectLogItemExternalService(IEcommerceRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository)
        {
            _objectHistoryLogItemRepository = objectHistoryLogItemRepository;
        }

        public async Task LogItems<T>(IEnumerable<T> models, bool logFullObjects = true) where T : class
        {
            if (models != null)
            {
                var type = typeof(T);
                var isDynamic = type.GetTypeInfo().IsSubclassOf(typeof(MappedObject));
                var isContentDataItem = type.GetTypeInfo().IsSubclassOf(typeof(ContentDataItem));
                var isLogEntity = type.GetTypeInfo().IsSubclassOf(typeof(LogEntity));
                var isEntity = type.GetTypeInfo().IsSubclassOf(typeof(Entity));
                var objectType = GetObjectType(type.Name, isDynamic, isEntity);

                List<ObjectHistoryLogItem> items = new List<ObjectHistoryLogItem>();
                foreach (var model in models)
                {
                    ObjectHistoryLogItem item;
                    if (isDynamic)
                    {
                        item = TransformForDynamic(model as MappedObject, objectType);
                    }
                    else if (isContentDataItem)
                    {
                        item = TransformForContentDataItem(model as ContentDataItem, objectType);
                    }
                    else if (isLogEntity)
                    {
                        item = TransformForLogEntity(model as LogEntity, objectType);
                    }
                    else if (isEntity)
                    {
                        item = TransformForEntity(model as Entity, objectType);
                    }
                    else
                    {
                        item = TransformForOther(model, objectType);
                    }
                    if (logFullObjects)
                    {
                        item.DataItem = new ObjectHistoryLogDataItem();
                        var settings = new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                            NullValueHandling = NullValueHandling.Ignore,
                            //ContractResolver = new PublicPropertiesResolver()
                        };
                        item.DataItem.Data = JsonConvert.SerializeObject(model, settings);
                    }
                    items.Add(item);
                }
                await _objectHistoryLogItemRepository.InsertGraphRangeAsync(items);
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

        private ObjectHistoryLogItem TransformForDynamic(MappedObject model, ObjectType objectType)
        {
            ObjectHistoryLogItem item = new ObjectHistoryLogItem
            {
                IdObjectType = (int) objectType,
                DateCreated = DateTime.Now,
                IdObject = model.Id,
                IdObjectStatus = model.StatusCode,
                IdEditedBy = model.IdEditedBy
            };
            if (objectType == ObjectType.Order)
            {
                if (model is OrderDynamic)
                {
                    OrderDynamic order = (OrderDynamic) model;
                    item.OptionalData = $"All:{(int?) order.OrderStatus},P:{(int?) order.POrderStatus},NP:{(int?) order.NPOrderStatus}";
                }
                if (model is OrderRefundDynamic)
                {
                    OrderRefundDynamic order = (OrderRefundDynamic) model;
                    item.OptionalData = $"All:{(int?) order.OrderStatus}";
                }
            }
            return item;
        }

        private ObjectHistoryLogItem TransformForContentDataItem(ContentDataItem model, ObjectType objectType)
        {
            ObjectHistoryLogItem item = new ObjectHistoryLogItem
            {
                IdObjectType = (int) objectType,
                DateCreated = DateTime.Now,
                IdObject = model.Id,
                IdObjectStatus = (int) model.StatusCode,
                IdEditedBy = model.UserId
            };
            return item;
        }

        private ObjectHistoryLogItem TransformForLogEntity(LogEntity model, ObjectType objectType)
        {
            ObjectHistoryLogItem item = new ObjectHistoryLogItem
            {
                IdObjectType = (int) objectType,
                DateCreated = DateTime.Now,
                IdObject = model.Id,
                IdObjectStatus = (int) model.StatusCode,
                IdEditedBy = model.IdEditedBy,
            };
            //TODO - add needed fields to general implementiotn of Entity
            return item;
        }

        private ObjectHistoryLogItem TransformForEntity(Entity model, ObjectType objectType)
        {
            ObjectHistoryLogItem item = new ObjectHistoryLogItem
            {
                IdObjectType = (int) objectType,
                DateCreated = DateTime.Now,
                IdObject = model.Id
            };
            return item;
        }

        private ObjectHistoryLogItem TransformForOther(object model, ObjectType objectType)
        {
            ObjectHistoryLogItem item = new ObjectHistoryLogItem
            {
                IdObjectType = (int) objectType,
                DateCreated = DateTime.Now
            };
            //TODO - add needed fields to general implementiotn of Entity
            return item;
        }

        private ObjectType GetObjectType(string name, bool isDynamic, bool isEntity)
        {
            ObjectType result = ObjectType.Unknown;
            if (isDynamic || isEntity)
            {
                if (isDynamic)
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