using System.Threading.Tasks;
using VitalChoice.Business.Services.VeraCore;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.ExportService.Services
{
    public interface IVeraCoreExportService
    {
        Task ExportOrder(OrderDynamic order, ExportSide exportSide);

        Task ExportRefund(OrderRefundDynamic order);
    }
}