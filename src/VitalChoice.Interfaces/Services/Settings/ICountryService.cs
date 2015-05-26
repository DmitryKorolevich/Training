using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Interfaces.Services.Settings
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
