using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Business.Services.Settings
{
    public class ObjectHistoryLogService : IObjectHistoryLogService
    {
        private readonly IEcommerceRepositoryAsync<ObjectHistoryLogItem> _objectHistoryLogItemRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly ILogger logger;

        public ObjectHistoryLogService(
            IEcommerceRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _objectHistoryLogItemRepository = objectHistoryLogItemRepository;
            _adminProfileRepository = adminProfileRepository;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<PagedList<ObjectHistoryLogItem>> GetObjectHistoryLogItems(ObjectHistoryLogItemsFilter filter)
        {
            if(filter.Paging==null)
            {
                filter.Paging = new Paging();
                filter.Paging.PageIndex = 0;
                filter.Paging.PageItemCount = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT;
            }
            Func<IQueryable<ObjectHistoryLogItem>, IOrderedQueryable<ObjectHistoryLogItem>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var toReturn = await _objectHistoryLogItemRepository.Query(p => p.IdObjectType == (int)filter.IdObjectType && p.IdObject == filter.IdObject).
                OrderBy(sortable).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            var profiles = await _adminProfileRepository.Query(p => toReturn.Items.Where(pp=>pp.IdEditedBy.HasValue).
                Select(pp=>pp.IdEditedBy).Contains(p.Id)).SelectAsync();
            foreach (var item in toReturn.Items)
            {
                foreach (var profile in profiles)
                {
                    if (item.IdEditedBy == profile.Id)
                    {
                        item.EditedBy = profile.AgentId;
                    }
                }
            }

            return toReturn;
        }

        public async Task<ObjectHistoryReportModel> GetObjectHistoryReport(ObjectHistoryLogItemsFilter filter)
        {
            ObjectHistoryReportModel toReturn = new ObjectHistoryReportModel();
            
            var ids = new List<int>();
            Func<IQueryable<ObjectHistoryLogItem>, IOrderedQueryable<ObjectHistoryLogItem>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            toReturn.Main =new ObjectHistoryLogListItemModel(
                (await _objectHistoryLogItemRepository.Query(p => p.IdObjectType == (int)filter.IdObjectType && p.IdObject == filter.IdObject).
                Include(p=>p.DataItem).
                OrderBy(sortable).
                SelectPageAsync(1, 1)).Items.FirstOrDefault());
            if(toReturn.Main!=null && toReturn.Main.IdEditedBy.HasValue)
            {
                ids.Add(toReturn.Main.IdEditedBy.Value);
            }

            if (filter.IdBeforeObjectHistoryLogItem.HasValue)
            {
                toReturn.Before = new ObjectHistoryLogListItemModel(
                    (await _objectHistoryLogItemRepository.Query(p => p.IdObjectHistoryLogItem == filter.IdBeforeObjectHistoryLogItem.Value).
                    Include(p => p.DataItem).SelectAsync(false)).FirstOrDefault());
                if (toReturn.Before != null && toReturn.Before.IdEditedBy.HasValue)
                {
                    ids.Add(toReturn.Before.IdEditedBy.Value);
                }
            }
            
            var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
            if (toReturn.Main != null && toReturn.Main.IdEditedBy.HasValue)
            {
                toReturn.Main.EditedBy = profiles.FirstOrDefault(p => p.Id == toReturn.Main.IdEditedBy)?.AgentId;
            }
            if (toReturn.Before != null && toReturn.Before.IdEditedBy.HasValue)
            {
                toReturn.Before.EditedBy = profiles.FirstOrDefault(p => p.Id == toReturn.Before.IdEditedBy)?.AgentId;
            }

            return toReturn;
        }
    }
}