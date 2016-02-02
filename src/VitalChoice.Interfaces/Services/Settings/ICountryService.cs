using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Transfer.Country;

namespace VitalChoice.Interfaces.Services.Settings
{
	public interface ICountryService
    {
        Task<ICollection<Country>> GetCountriesAsync(CountryFilter filter = null);
        Task<Country> GetCountryAsync(int id);
        Task<bool> UpdateCountriesOrderAsync(ICollection<Country> model);
        Task<Country> UpdateCountryAsync(Country model);
        Task<bool> DeleteCountryAsync(int id);

        Task<State> UpdateStateAsync(State model);
        Task<bool> DeleteStateAsync(int id);
    }
}