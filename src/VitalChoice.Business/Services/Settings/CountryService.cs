﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services.Settings
{
    public class CountryService : ICountryService
    {
        private const int CODE_MAX_SYMBOLS_COUNT= 3;

        private readonly IEcommerceRepositoryAsync<Country> countryRepository;
        private readonly IEcommerceRepositoryAsync<State> stateRepository;
        private readonly ILogger logger;

        public CountryService(IEcommerceRepositoryAsync<Country> countryRepository,
            IEcommerceRepositoryAsync<State> stateRepository, ILoggerFactory loggerProvider)
        {
            this.countryRepository = countryRepository;
            this.stateRepository = stateRepository;
            logger = loggerProvider.CreateLogger<CountryService>();
        }

        public async Task<ICollection<Country>> GetCountriesAsync(CountryFilter filter = null)
        {
            Expression<Func<Country, bool>> countryQuery = p => p.StatusCode != RecordStatusCode.Deleted;
            Expression<Func<State, bool>> stateQuery = p => p.StatusCode != RecordStatusCode.Deleted;
            if (filter?.ActiveOnly == true)
            {
                countryQuery = countryQuery.And(x => x.StatusCode == RecordStatusCode.Active);
                stateQuery = stateQuery.And(x => x.StatusCode == RecordStatusCode.Active);
            }
            if (!string.IsNullOrEmpty(filter?.CountryCode))
            {
                countryQuery = countryQuery.And(x => x.CountryCode == filter.CountryCode);
            }

            var countries = await countryRepository.Query(countryQuery).OrderBy(e => e.OrderBy(c => c.Order)).SelectAsync(false);
            var states = (await stateRepository.Query(stateQuery)
                .OrderBy(e => e.OrderBy(s => s.Order))
                .SelectAsync(false))
                .GroupBy(s => s.CountryCode)
                .ToDictionary(s => s.Key, s => s.ToList());

            foreach (var item in countries)
            {
                List<State> stateList;
                item.States = states.TryGetValue(item.CountryCode, out stateList) ? stateList : new List<State>();
            }

            return countries;
        }

        public ICollection<Country> GetCountries(CountryFilter filter = null)
        {
            Expression<Func<Country, bool>> countryQuery = p => p.StatusCode != RecordStatusCode.Deleted;
            Expression<Func<State, bool>> stateQuery = p => p.StatusCode != RecordStatusCode.Deleted;
            if (filter?.ActiveOnly == true)
            {
                countryQuery = countryQuery.And(x => x.StatusCode == RecordStatusCode.Active);
                stateQuery = stateQuery.And(x => x.StatusCode == RecordStatusCode.Active);
            }
            if (!string.IsNullOrEmpty(filter?.CountryCode))
            {
                countryQuery = countryQuery.And(x => x.CountryCode == filter.CountryCode);
            }

            var countries = countryRepository.Query(countryQuery).OrderBy(e => e.OrderBy(c => c.Order)).Select(false);
            var states = stateRepository.Query(stateQuery)
                .OrderBy(e => e.OrderBy(s => s.Order))
                .Select(false)
                .GroupBy(s => s.CountryCode)
                .ToDictionary(s => s.Key, s => s.ToList());

            foreach (var item in countries)
            {
                List<State> stateList;
                item.States = states.TryGetValue(item.CountryCode, out stateList) ? stateList : new List<State>();
            }

            return countries;
        }

        public async Task<Country> GetCountryAsync(int id)
        {
            Country toReturn = (await countryRepository.Query(x=>x.Id==id).SelectFirstOrDefaultAsync(false));
            if (toReturn != null)
            {
                toReturn.States = await stateRepository.Query(x=>x.CountryCode == toReturn.CountryCode).SelectAsync(false);
            }
            return toReturn;
        }

        public async Task<bool> UpdateCountriesOrderAsync(ICollection<Country> model)
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

            var dbCountries = await countryRepository.Query(p=>p.StatusCode != RecordStatusCode.Deleted).SelectAsync(true);
            foreach (var dbCountry in dbCountries)
            {
                var country = model.FirstOrDefault(p => p.Id == dbCountry.Id);
                if (country != null)
                {
                    dbCountry.Order = country.Order;
                }
            }
            toReturn = await countryRepository.UpdateRangeAsync(dbCountries);

            var dbStates = await stateRepository.Query(p => p.StatusCode != RecordStatusCode.Deleted).SelectAsync(true);
            foreach (var dbState in dbStates)
            {
                var country = model.FirstOrDefault(p => p.CountryCode == dbState.CountryCode);
                var state = country?.States.FirstOrDefault(p => p.Id == dbState.Id);
                if (state != null)
                {
                    dbState.Order = state.Order;
                }
            }
            toReturn = toReturn && await stateRepository.UpdateRangeAsync(dbStates);

            return toReturn;
        }

        public async Task<Country> UpdateCountryAsync(Country model)
        {
            Country dbItem = null;
            if (model.Id==0)
            {
                dbItem = new Country();

                var countries = await countryRepository.Query(p =>p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);
                if (countries.Count != 0)
                {
                    dbItem.Order = countries.Max(p => p.Order) + 1;
                }
            }
            else
            {
                dbItem = (await countryRepository.Query(p=>p.Id==model.Id).SelectFirstOrDefaultAsync(false));
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                if (dbItem.CountryCode != model.CountryCode)
                {
                    var idDbItem = dbItem.Id;
                    var codeDublicatesExist = await countryRepository.Query(p => p.CountryCode == model.CountryCode
                                                                                 && p.Id != idDbItem &&
                                                                                 p.StatusCode !=
                                                                                 RecordStatusCode.Deleted)
                        .SelectAnyAsync();
                    if (codeDublicatesExist)
                    {
                        throw new AppValidationException("CountryCode",
                            "Country with the same country code already exists, please use a unique country code.");
                    }
                }

                dbItem.CountryCode = model.CountryCode.ToUpper();
                if(dbItem.CountryCode.Length> CODE_MAX_SYMBOLS_COUNT)
                {
                    dbItem.CountryCode = dbItem.CountryCode.Substring(0, CODE_MAX_SYMBOLS_COUNT);
                }
                dbItem.CountryName = model.CountryName;
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.IdVisibility = model.IdVisibility;

                if (model.Id == 0)
                {
                    await countryRepository.InsertAsync(dbItem);
                }
                else
                {
                    await countryRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await countryRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));
            if (dbItem != null)
            {
                string message = String.Empty;
                var statesExist =
                    await
                        stateRepository.Query(
                            p => p.CountryCode == dbItem.CountryCode && p.StatusCode != RecordStatusCode.Deleted)
                            .SelectAnyAsync();
                if (statesExist)
                {
                    message += "Country with states can't be deleted. " + Environment.NewLine;
                }
                
                if (!String.IsNullOrEmpty(message))
                {
                    throw new AppValidationException(message);
                }

                try
                {
                    countryRepository.Delete(dbItem);
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException != null && e.InnerException is SqlException && (e.InnerException as SqlException).Number== SqlConstants.ERROR_CODE_FOREIGN_KEY_CONFLICT)
                    {
                        throw new AppValidationException(string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.DenyDeleteInUseItem], "Country"));
                    }
                }

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
                dbItem = (await stateRepository.Query(p => p.Id == model.Id).SelectFirstOrDefaultAsync(false));
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
                    await stateRepository.InsertAsync(dbItem);
                }
                else
                {
                    await stateRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await stateRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));
            if (dbItem != null)
            {
                try
                {
                    stateRepository.Delete(dbItem);
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException != null && e.InnerException is SqlException && (e.InnerException as SqlException).Number == SqlConstants.ERROR_CODE_FOREIGN_KEY_CONFLICT)
                    {
                        throw new AppValidationException(string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.DenyDeleteInUseItem], "State"));
                    }
                }

                toReturn = true;
            }
            return toReturn;
        }
    }
}