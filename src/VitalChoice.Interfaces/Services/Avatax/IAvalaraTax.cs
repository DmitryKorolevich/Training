using System.Threading.Tasks;
using VitalChoice.Domain.Avatax;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Interfaces.Services.Avatax
{
    public interface IAvalaraTax
    {
        Task<bool> CancelTax(string orderCode);
        Task<bool> CommitTax(int idOrder, TaxGetType taxGetType = TaxGetType.UseBoth);
        Task<decimal> GetTax(OrderContext order, TaxGetType taxGetType = TaxGetType.UseBoth);
    }
}