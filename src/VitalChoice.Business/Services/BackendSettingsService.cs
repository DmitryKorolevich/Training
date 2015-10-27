using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class BackendSettingsService : IBackendSettingsService
    {
        private readonly IOptions<AppOptions> _appOptions;
        private readonly IEcommerceRepositoryAsync<Country> _countryRepository;
        private readonly IEcommerceRepositoryAsync<State> _stateRepository;
        private readonly ILogger _logger;

        public BackendSettingsService(
            IOptions<AppOptions> appOptions,
            IEcommerceRepositoryAsync<Country> countryRepository,
            IEcommerceRepositoryAsync<State> stateRepository,
            ILoggerProviderExtended loggerProvider)
        {
            _appOptions = appOptions;
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
            }
            return toReturn;
        }
    }
}
