using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface IObjectHistoryLogService
    {
        Task<PagedList<ObjectHistoryLogItem>> GetObjectHistoryLogItems(ObjectHistoryLogItemsFilter filter);

        Task<ObjectHistoryReportModel> GetObjectHistoryReport(ObjectHistoryLogItemsFilter filter);
    }
}