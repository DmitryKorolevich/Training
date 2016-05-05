using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Interfaces.Services.Avatax
{
    public interface IAvalaraTax
    {
        Task<bool> CancelTax(string orderCode);
        Task<bool> CommitTax(int idOrder, TaxGetType taxGetType = TaxGetType.UseBoth);
        Task<decimal> GetTax(OrderDataContext context, TaxGetType taxGetType = TaxGetType.UseBoth);
        Task<decimal> GetTax(OrderRefundDataContext context, TaxGetType taxGetType = TaxGetType.UseBoth);
    }
}