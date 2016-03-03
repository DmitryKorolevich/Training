using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using System.Linq;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class BackendSettingsService : IBackendSettingsService
    {
        private readonly IEcommerceRepositoryAsync<Country> _countryRepository;
        private readonly IEcommerceRepositoryAsync<State> _stateRepository;
        private readonly ILogger _logger;

        public BackendSettingsService(
            IEcommerceRepositoryAsync<Country> countryRepository,
            IEcommerceRepositoryAsync<State> stateRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public Country GetDefaultCountry()
        {
            var toReturn = _countryRepository.Query(p => p.CountryCode == "US").Select(false).FirstOrDefault();
            if(toReturn != null)
            {
                toReturn.States = _stateRepository.Query(p => p.CountryCode == "US").Select(false);
                toReturn.States = toReturn.States.OrderBy(p => p.Order).ToList();
            }
            return toReturn;
        }
    }
}
