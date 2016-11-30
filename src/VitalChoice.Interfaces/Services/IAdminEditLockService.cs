using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;

namespace VitalChoice.Interfaces.Services
{
    public interface IAdminEditLockService
    {
        bool AgentEditLockPing(EditLockPingModel model, string browserUserAgent);

        EditLockRequestModel AgentEditLockRequest(EditLockRequestModel model, string browserUserAgent);

        void ExportOrderEditLockRequest(int idOrder, string lockMessageTitle, string lockMessageBody);

        void ExportOrderEditLockRelease(int idOrder);

        bool GetIsOrderLocked(int idOrder);
    }
}