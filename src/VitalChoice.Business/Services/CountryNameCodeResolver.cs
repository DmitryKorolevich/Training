using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services
{
    public class CountryNameCodeResolver : ICountryNameCodeResolver
    {
        public CountryNameCodeResolver(ICountryService countryService)
        {
            var countries = countryService.GetCountriesAsync().GetAwaiter().GetResult();
            _coutries = countries.ToDictionary(c => c.CountryCode, c => c.Id, StringComparer.OrdinalIgnoreCase);
            _states = countries.ToDictionary(c => c.CountryCode,
                c => c.States.ToDictionary(s => s.StateCode, s => s.Id, StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
            _coutryCodes = countries.ToDictionary(c => c.Id);
            _stateCodes = countries.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
        }

        private readonly Dictionary<string, int> _coutries;

        private readonly Dictionary<string, Dictionary<string, int>> _states;

        private readonly Dictionary<int, Country> _coutryCodes;

        private readonly Dictionary<int, Dictionary<int, State>> _stateCodes;

        public bool IsState(AddressDynamic address, string countryCode, string stateCode)
        {
            return _states.GetStateId(countryCode, stateCode) == address?.IdState;
        }

        public bool IsCountry(AddressDynamic address, string countryCode)
        {
            return _coutries.GetCountryId(countryCode) == address?.IdCountry;
        }

        public string GetCountryCode(int idCountry)
        {
            return _coutryCodes.GetCountry(idCountry)?.CountryCode;
        }

        public string GetStateCode(int idCountry, int idState)
        {
            return _stateCodes.GetState(idCountry, idState)?.StateCode;
        }

        public string GetStateCode(AddressDynamic address)
        {
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateCode;
        }

        public string GetStateName(AddressDynamic address)
        {
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateName;
        }

        public string GetCountryCode(AddressDynamic address)
        {
            return _coutryCodes.GetCountry(address?.IdCountry ?? 0)?.CountryCode;
        }

        public string GetCountryName(AddressDynamic address)
        {
            return _coutryCodes.GetCountry(address?.IdCountry ?? 0)?.CountryName;
        }

        public string GetRegionOrStateCode(AddressDynamic address)
        {
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateCode ?? address?.County;
        }

        public string GetRegionOrStateName(AddressDynamic address)
        {
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateName ?? address?.County;
        }
    }
}
