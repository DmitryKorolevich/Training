﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Data.Services;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Services.Settings
{
    public class ObjectLogItemExternalService : IObjectLogItemExternalService
    {
        private readonly IEcommerceRepositoryAsync<ObjectHistoryLogItem> _objectHistoryLogItemRepository;
        private readonly ILogger _logger;

        public ObjectLogItemExternalService(
            IEcommerceRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _objectHistoryLogItemRepository = objectHistoryLogItemRepository;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task LogItems(ICollection<object> models, bool logFullObjects)
        {
            if (models != null && models.Count > 0)
            {
                var type = models.First().GetType();
                var isDynamic = type.GetTypeInfo().IsSubclassOf(typeof(MappedObject));
                var isContentDataItem = type.GetTypeInfo().IsSubclassOf(typeof(ContentDataItem));
                var isEntity = type.GetTypeInfo().IsSubclassOf(typeof(Entity));
                var objectType = GetObjectType(type.Name, isDynamic, isEntity);

                List<ObjectHistoryLogItem> items = new List<ObjectHistoryLogItem>();
                foreach (var model in models)
                {
                    ObjectHistoryLogItem item = new ObjectHistoryLogItem();
                    item.IdObjectType = (int)objectType;
                    item.DateCreated = DateTime.Now;
                    if (isDynamic)
                    {
                        TransformForDynamic(item, (MappedObject)model, objectType);
                    } else if (isContentDataItem)
                    {
                        TransformForContentDataItem(item, (ContentDataItem)model, objectType);
                    } else if (isEntity)
                    {
                        TransformForEntity(item, (Entity)model, objectType);
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

        private ObjectHistoryLogItem TransformForDynamic(ObjectHistoryLogItem item, MappedObject model, ObjectType objectType)
        {
            item.IdObject = model.Id;
            item.IdObjectStatus = model.StatusCode;
            item.IdEditedBy = model.IdEditedBy;
            if(objectType == ObjectType.Order)
            {
                OrderDynamic order = (OrderDynamic)model;
                item.OptionalData = $"All:{(int?)order.OrderStatus},P:{(int?)order.POrderStatus},NP:{(int?)order.NPOrderStatus}";
            }
            return item;
        }

        private ObjectHistoryLogItem TransformForContentDataItem(ObjectHistoryLogItem item, ContentDataItem model, ObjectType objectType)
        {
            item.IdObject = model.Id;
            item.IdObjectStatus = (int)model.StatusCode;
            item.IdEditedBy = model.UserId;
            return item;
        }

        private ObjectHistoryLogItem TransformForEntity(ObjectHistoryLogItem item, Entity model, ObjectType objectType)
        {
            item.IdObject = model.Id;
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