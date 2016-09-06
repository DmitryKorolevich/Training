using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.CreditCards.Services
{
    public interface IVeraCoreExportService
    {
        Task ExportOrder(OrderDynamic order, ExportSide exportSide);

        Task ExportRefund(OrderRefundDynamic order);
    }
}