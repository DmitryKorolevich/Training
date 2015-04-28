using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Domain.Transfer.Product;

namespace VitalChoice.Business.Services.Contracts.Settings
{
	public interface ICountryService
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<bool> UpdateCountriesOrderAsync(IEnumerable<Country> model);
        Task<Country> UpdateCountryAsync(Country model);
        Task<bool> DeleteCountryAsync(int id);

        Task<State> UpdateStateAsync(State model);
        Task<bool> DeleteStateAsync(int id);
    }
}
