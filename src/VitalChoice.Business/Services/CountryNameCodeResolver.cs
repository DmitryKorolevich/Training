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
        private readonly Lazy<ICollection<Country>> _countrySource;

        public CountryNameCodeResolver(ICountryService countryService)
        {
            _countrySource = new Lazy<ICollection<Country>>(() => countryService.GetCountriesAsync().GetAwaiter().GetResult());
        }

        private Dictionary<string, int> _countries;

        private Dictionary<string, Dictionary<string, int>> _states;

        private Dictionary<int, Country> _coutryCodes;

        private Dictionary<int, Dictionary<int, State>> _stateCodes;

        public bool IsState(int idState, string countryCode, string stateCode)
        {
            if (_states == null)
            {
                _states = _countrySource.Value.ToDictionary(c => c.CountryCode,
                    c => c.States.ToDictionary(s => s.StateCode, s => s.Id, StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase);
            }
            return _states.GetStateId(countryCode, stateCode) == idState;
        }

        public bool IsState(AddressDynamic address, string countryCode, string stateCode)
        {
            if (_states == null)
            {
                _states = _countrySource.Value.ToDictionary(c => c.CountryCode,
                    c => c.States.ToDictionary(s => s.StateCode, s => s.Id, StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase);
            }
            return _states.GetStateId(countryCode, stateCode) == address?.IdState;
        }

        public bool IsCountry(AddressDynamic address, string countryCode)
        {
            if (_countries == null)
            {
                _countries = _countrySource.Value.ToDictionary(c => c.CountryCode, c => c.Id, StringComparer.OrdinalIgnoreCase);
            }
            return _countries.GetCountryId(countryCode) == address?.IdCountry;
        }

        public string GetCountryCode(int idCountry)
        {
            if (_coutryCodes == null)
            {
                _coutryCodes = _countrySource.Value.ToDictionary(c => c.Id);
            }
            return _coutryCodes.GetCountry(idCountry)?.CountryCode;
        }

        public string GetStateCode(int idCountry, int idState)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _stateCodes.GetState(idCountry, idState)?.StateCode;
        }

        public string GetStateCode(AddressDynamic address)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateCode;
        }

        public string GetStateName(AddressDynamic address)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateName;
        }

        public string GetCountryCode(AddressDynamic address)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _coutryCodes.GetCountry(address?.IdCountry ?? 0)?.CountryCode;
        }

        public string GetCountryName(AddressDynamic address)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _coutryCodes.GetCountry(address?.IdCountry ?? 0)?.CountryName;
        }

        public string GetRegionOrStateCode(AddressDynamic address)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateCode ?? address?.County;
        }

        public string GetRegionOrStateName(AddressDynamic address)
        {
            if (_stateCodes == null)
            {
                _stateCodes = _countrySource.Value.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            }
            return _stateCodes.GetState(address?.IdCountry ?? 0, address?.IdState ?? 0)?.StateName ?? address?.County;
        }
    }
}