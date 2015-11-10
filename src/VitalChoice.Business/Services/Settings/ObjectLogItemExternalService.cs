using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Domain.Transfer.Settings;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Constants;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Data.Services;
using Newtonsoft.Json;
using VitalChoice.Domain.Entities.eCommerce;
using System.Reflection;
using VitalChoice.Domain;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;

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
                    }
                    if (isEntity)
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
                item.IdObjectStatus = (int)((OrderDynamic)model).OrderStatus;
            }
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