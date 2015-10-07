using System.Threading.Tasks;
using VitalChoice.Domain.Avatax;

namespace Avalara.Avatax.Rest.Services
{
    public interface ITaxService
    {
        Task<CancelTaxResult> CancelTax(CancelTaxRequest cancelTaxRequest);
        Task<GeoTaxResult> EstimateTax(decimal latitude, decimal longitude, decimal saleAmount);
        Task<GetTaxResult> GetTax(GetTaxRequest req);
        Task<GeoTaxResult> Ping();
    }
}