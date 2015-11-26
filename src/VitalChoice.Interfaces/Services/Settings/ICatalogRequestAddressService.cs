using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.CatalogRequests;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface ICatalogRequestAddressService
    {
        Task<ICollection<CatalogRequestAddressListItemModel>> GetCatalogRequestsAsync();

        Task<bool> DeleteCatalogRequestsAsync();
    }
}
