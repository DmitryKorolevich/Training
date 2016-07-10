using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Infrastructure.Services
{
	public interface IObjectHistoryLogService
    {
        Task<PagedList<ObjectHistoryItem>> GetObjectHistoryLogItems(ObjectHistoryLogItemsFilter filter);

        Task<ObjectHistoryReportModel> GetObjectHistoryReport(ObjectHistoryLogItemsFilter filter);
    }
}