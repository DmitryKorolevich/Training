using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Country;
using VitalChoice.Domain.Transfer.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface IObjectHistoryLogService
    {
        Task<PagedList<ObjectHistoryLogItem>> GetObjectHistoryLogItems(ObjectHistoryLogItemsFilter filter);

        Task<ObjectHistoryReportModel> GetObjectHistoryReport(ObjectHistoryLogItemsFilter filter);
    }
}