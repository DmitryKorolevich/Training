using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Interfaces.Services.Avatax
{
    public interface IAvalaraTax
    {
        Task<bool> CancelTax(string orderCode);
        Task<bool> CommitTax(string orderCode, int? idState, TaxGetType taxGetType = TaxGetType.UseBoth);

        Task<decimal> GetTax<T>(BaseOrderContext<T> context, TaxGetType taxGetType = TaxGetType.UseBoth)
            where T : ItemOrdered;

        //Task<decimal> GetTax(OrderRefundDataContext context, TaxGetType taxGetType = TaxGetType.UseBoth);
    }
}