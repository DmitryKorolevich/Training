using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Interfaces.Services
{
    public interface ICountryNameCodeResolver
    {
        Country GetCountryByName(string name);
        Country GetCountryByCode(string code);
        State GetStateByName(int idCountry, string name);
        State GetStateByCode(int idCountry, string code);
        bool IsState(int idState, string countryCode, string stateCode);
        bool IsState(AddressDynamic address, string countryCode, string stateCode);
        bool IsCountry(AddressDynamic address, string countryCode);
        string GetCountryCode(int idCountry);
        string GetStateCode(int idCountry, int idState);
        string GetStateCode(AddressDynamic address);
        string GetStateName(AddressDynamic address);
        string GetStateName(int idCountry, int idState);
        string GetCountryCode(AddressDynamic address);
        string GetCountryName(AddressDynamic address);
        string GetCountryName(int idCountry);
        string GetRegionOrStateCode(AddressDynamic address);
        string GetRegionOrStateName(AddressDynamic address);
    }
}