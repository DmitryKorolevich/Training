using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Infrastructure.Services
{
    public class ObjectHistoryLogService : IObjectHistoryLogService
    {
        private readonly IEcommerceRepositoryAsync<ObjectHistoryLogItem> _objectHistoryLogItemRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;

        public ObjectHistoryLogService(
            IEcommerceRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository,
            IRepositoryAsync<AdminProfile> adminProfileRepository)
        {
            _objectHistoryLogItemRepository = objectHistoryLogItemRepository;
            _adminProfileRepository = adminProfileRepository;
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
            Func<IQueryable<ObjectHistoryLogItem>, IOrderedQueryable<ObjectHistoryLogItem>> sortable =
                x => x.OrderByDescending(y => y.DateCreated);
            var toReturn =
                await
                    _objectHistoryLogItemRepository.Query(p => p.IdObjectType == (int) filter.IdObjectType && p.IdObject == filter.IdObject)
                        .OrderBy(sortable)
                        .SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, false);
            var adminIds = toReturn.Items.Where(pp => pp.IdEditedBy.HasValue).
                Select(pp => pp.IdEditedBy.Value).Distinct().ToList();
            var profiles = await _adminProfileRepository.Query(p => adminIds.Contains(p.Id)).SelectAsync(false);
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

            return new PagedList<ObjectHistoryItem>(toReturn.Items.Select(i => new ObjectHistoryItem(i)).ToList(), toReturn.Count);
        }

        public async Task<ObjectHistoryReportModel> GetObjectHistoryReport(ObjectHistoryLogItemsFilter filter)
        {
            ObjectHistoryReportModel toReturn = new ObjectHistoryReportModel();

            var ids = new HashSet<int>();
            Func<IQueryable<ObjectHistoryLogItem>, IOrderedQueryable<ObjectHistoryLogItem>> sortable =
                x => x.OrderByDescending(y => y.DateCreated);
            var mainEntity = await
                _objectHistoryLogItemRepository.Query(p => p.IdObjectType == (int) filter.IdObjectType && p.IdObject == filter.IdObject)
                    .Include(p => p.DataItem)
                    .OrderBy(sortable)
                    .SelectFirstOrDefaultAsync(false);
            toReturn.Main = new ObjectHistoryLogListItemModel(mainEntity);
            if (mainEntity?.IdEditedBy != null)
            {
                ids.Add(mainEntity.IdEditedBy.Value);
            }

            ObjectHistoryLogItem entityCompareTo = null;

            if (!string.IsNullOrEmpty(filter.DataReferenceId))
            {
                entityCompareTo = await
                    _objectHistoryLogItemRepository.Query(p => p.IdObjectHistoryLogItem == long.Parse(filter.DataReferenceId))
                        .Include(p => p.DataItem)
                        .SelectFirstOrDefaultAsync(false);
                toReturn.Before = new ObjectHistoryLogListItemModel(entityCompareTo);
                if (entityCompareTo?.IdEditedBy != null)
                {
                    ids.Add(entityCompareTo.IdEditedBy.Value);
                }
            }

            var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
            if (mainEntity?.IdEditedBy != null)
            {
                toReturn.Main.EditedBy = profiles.FirstOrDefault(p => p.Id == mainEntity.IdEditedBy)?.AgentId;
            }
            if (entityCompareTo?.IdEditedBy != null)
            {
                toReturn.Before.EditedBy = profiles.FirstOrDefault(p => p.Id == entityCompareTo.IdEditedBy)?.AgentId;
            }

            return toReturn;
        }
    }
}