﻿using Microsoft.Framework.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Business.Services.Contracts.Settings;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Business.Services.Impl.Settings
{
    public class CountryService : ICountryService
    {
        private const int CODE_MAX_SYMBOLS_COUNT= 3;

        private readonly IRepositoryAsync<Country> countryRepository;
        private readonly IRepositoryAsync<State> stateRepository;
        private readonly ILogger logger;

        public CountryService(IRepositoryAsync<Country> countryRepository, IRepositoryAsync<State> stateRepository)
        {
            this.countryRepository = countryRepository;
            this.stateRepository = stateRepository;
            logger = LoggerService.GetDefault();
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            List<Country> toReturn = null;
            toReturn = (await countryRepository.Query(p => p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).ToList();
            var states = (await stateRepository.Query(p => p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).ToList();
            toReturn = toReturn.OrderBy(p => p.Order).ToList();
            foreach(var item in toReturn)
            {
                item.States = new List<State>();
                var countryStates = states.Where(p => p.CountryCode == item.CountryCode).OrderBy(p => p.Order).ToList();
                foreach (var countryState in countryStates)
                {
                    item.States.Add(countryState);
                }
            }

            return toReturn;
        }

        public async Task<bool> UpdateCountriesOrderAsync(IEnumerable<Country> model)
        {
            bool toReturn = false;
            int i = 0;
            foreach (var country in model)
            {
                country.Order = i;
                if (country.States != null)
                {
                    int j = 0;
                    foreach (var state in country.States)
                    {
                        state.Order = j;
                        j++;
                    }
                }
                i++;
            }

            var dbCountries = (await countryRepository.Query(p=>p.StatusCode != RecordStatusCode.Deleted).SelectAsync()).ToList();
            foreach (var dbCountry in dbCountries)
            {
                var country = model.Where(p => p.Id == dbCountry.Id).FirstOrDefault();
                if (country != null)
                {
                    dbCountry.Order = country.Order;
                }
            }
            toReturn = await countryRepository.UpdateRangeAsync(dbCountries);

            var dbStates = (await stateRepository.Query(p => p.StatusCode != RecordStatusCode.Deleted).SelectAsync()).ToList();
            foreach (var dbState in dbStates)
            {
                var country = model.Where(p => p.CountryCode == dbState.CountryCode).FirstOrDefault();
                if (country != null)
                {
                    var state = country.States.Where(p => p.Id == dbState.Id).FirstOrDefault();
                    if (state != null)
                    {
                        dbState.Order = state.Order;
                    }
                }
            }
            toReturn = await stateRepository.UpdateRangeAsync(dbStates);

            return toReturn;
        }

        public async Task<Country> UpdateCountryAsync(Country model)
        {
            Country dbItem = null;
            if (model.Id==0)
            {
                dbItem = new Country();

                var countries = (await countryRepository.Query(p =>p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).ToList();
                if (countries.Count != 0)
                {
                    dbItem.Order = countries.Max(p => p.Order) + 1;
                }
            }
            else
            {
                dbItem = (await countryRepository.Query(p=>p.Id==model.Id).SelectAsync(false)).FirstOrDefault();
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                if (dbItem.CountryCode != model.CountryCode)
                {
                    var codeDublicatesExist = await countryRepository.Query(p => p.CountryCode == model.CountryCode 
                        && p.Id!=dbItem.Id && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                    if (codeDublicatesExist)
                    {
                        throw new AppValidationException("CountryCode", "Country with the same country code already exists, please use a unique country code.");
                    }
                }

                dbItem.CountryCode = model.CountryCode.ToUpper();
                if(dbItem.CountryCode.Length> CODE_MAX_SYMBOLS_COUNT)
                {
                    dbItem.CountryCode = dbItem.CountryCode.Substring(0, CODE_MAX_SYMBOLS_COUNT);
                }
                dbItem.CountryName = model.CountryName;
                if (model.StatusCode != RecordStatusCode.Deleted)
                {
                    dbItem.StatusCode = model.StatusCode;
                }

                if (model.Id == 0)
                {
                    dbItem = await countryRepository.InsertAsync(dbItem);
                }
                else
                {
                    dbItem = await countryRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await countryRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                string message = String.Empty;
                var statesExist = (await stateRepository.Query(p=>p.CountryCode==dbItem.CountryCode && p.StatusCode!=RecordStatusCode.Deleted).SelectAnyAsync());
                if (statesExist)
                {
                    message += "Country with states can't be deleted. " + Environment.NewLine;
                }
                
                if (!String.IsNullOrEmpty(message))
                {
                    throw new AppValidationException(message);
                }
                
                countryRepository.Delete(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        public async Task<State> UpdateStateAsync(State model)
        {
            State dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new State();
                dbItem.CountryCode = model.CountryCode?.ToUpper();
                if (String.IsNullOrEmpty(dbItem.CountryCode))
                {
                    throw new AppValidationException("Country with the given country code doesn't exist.");
                }
                else
                {
                    var exist = await countryRepository.Query(p => p.CountryCode == dbItem.CountryCode &&
                        p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                    if (!exist)
                    {
                        throw new AppValidationException("Country with the given country code doesn't exist.");
                    }
                }

                var states = (await stateRepository.Query(p => p.CountryCode == dbItem.CountryCode && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).ToList();
                if (states.Count != 0)
                {
                    dbItem.Order = states.Max(p => p.Order) + 1;
                }
            }
            else
            {
                dbItem = (await stateRepository.Query(p => p.Id == model.Id).SelectAsync(false)).FirstOrDefault();
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                if (dbItem.StateCode != model.StateCode)
                {
                    var codeDublicatesExist = await stateRepository.Query(p => p.StateCode == model.StateCode && p.CountryCode == model.CountryCode
                        && p.Id != dbItem.Id && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                    if (codeDublicatesExist)
                    {
                        throw new AppValidationException("StateCode", "State / Province with the same state / province code already exists, please use a unique state / province code.");
                    }
                }

                dbItem.StateCode = model.StateCode.ToUpper();
                if (dbItem.StateCode.Length > CODE_MAX_SYMBOLS_COUNT)
                {
                    dbItem.CountryCode = dbItem.StateCode.Substring(0, CODE_MAX_SYMBOLS_COUNT);
                }
                dbItem.StateName = model.StateName;
                if (model.StatusCode != RecordStatusCode.Deleted)
                {
                    dbItem.StatusCode = model.StatusCode;
                }

                if (model.Id == 0)
                {
                    dbItem = await stateRepository.InsertAsync(dbItem);
                }
                else
                {
                    dbItem = await stateRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await stateRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                stateRepository.Delete(dbItem);

                toReturn = true;
            }
            return toReturn;
        }
    }
}