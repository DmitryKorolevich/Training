using System.Collections.Generic;
using System.Linq;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Helpers
{
    public static class CountrySearchExtension
    {
        public static int GetStateId(this Dictionary<string, Dictionary<string, int>> collection, string countryCode,
            string stateCode)
        {
            Dictionary<string, int> states;
            if (collection.TryGetValue(countryCode, out states))
            {
                int result;
                states.TryGetValue(stateCode, out result);
                return result;
            }
            return 0;
        }

        public static int GetCountryId(this Dictionary<string, int> collection, string countryCode)
        {
            int result;
            collection.TryGetValue(countryCode, out result);
            return result;
        }

        public static Country GetCountry(this Dictionary<int, Country> countries, int id)
        {
            Country result;
            countries.TryGetValue(id, out result);
            return result;
        }

        public static State GetState(this Dictionary<int, Dictionary<int, State>> states, int idCountry, int idState)
        {
            Dictionary<int, State> country;
            if (states.TryGetValue(idCountry, out country))
            {
                State result;
                country.TryGetValue(idState, out result);
                return result;
            }
            return null;
        }

    }
}