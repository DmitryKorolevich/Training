using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Interfaces.Services
{
    public interface ICountryNameCodeResolver
    {
        bool IsState(int idState, string countryCode, string stateCode);
        bool IsState(AddressDynamic address, string countryCode, string stateCode);
        bool IsCountry(AddressDynamic address, string countryCode);
        string GetCountryCode(int idCountry);
        string GetStateCode(int idCountry, int idState);
        string GetStateCode(AddressDynamic address);
        string GetStateName(AddressDynamic address);
        string GetCountryCode(AddressDynamic address);
        string GetCountryName(AddressDynamic address);
        string GetRegionOrStateCode(AddressDynamic address);
        string GetRegionOrStateName(AddressDynamic address);
    }
}
