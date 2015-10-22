using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
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

        public async Task LogItems(List<ObjectHistoryLogItem> items, bool logFullObjects)
        {
            if (logFullObjects)
            {
                foreach (var item in items)
                {
                    item.DataItem = new ObjectHistoryLogDataItem();
                    var settings = new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                    };
                    item.DataItem.Data = JsonConvert.SerializeObject(item.LogObject, settings);
                }
            }
            await _objectHistoryLogItemRepository.InsertGraphRangeAsync(items);
        }
    }
}