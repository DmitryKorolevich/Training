using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.CatalogRequests;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.CatalogRequests;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.DynamicData.Base;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;

namespace VitalChoice.Business.Services.Settings
{
    public class CatalogRequestAddressService : EcommerceDynamicService<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue>, 
        ICatalogRequestAddressService
    {
        private readonly IEcommerceDynamicService<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> _catalogRequestAddressService;
        private readonly IDynamicMapper<AddressDynamic, CatalogRequestAddress> _catalogRequestAddressMapper;
        private readonly ICountryService _countryService;
        private readonly IEcommerceRepositoryAsync<CatalogRequestAddress> _catalogRequestAddressRepository;
        private readonly ILogger logger;

        public CatalogRequestAddressService(
            IDynamicMapper<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> mapper,
            ICountryService countryService,
            IEcommerceRepositoryAsync<CatalogRequestAddress> catalogRequestAddressRepository,
            IEcommerceRepositoryAsync<CatalogRequestAddressOptionValue> catalogRequestAddressValueRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            DirectMapper<CatalogRequestAddress> directMapper, 
            DynamicExpressionVisitor queryVisitor,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended loggerProvider)
            : base(
                mapper, catalogRequestAddressRepository, catalogRequestAddressValueRepository,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor)
        {
            _catalogRequestAddressRepository = catalogRequestAddressRepository;
            _countryService = countryService;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<ICollection<CatalogRequestAddressListItemModel>> GetCatalogRequestsAsync()
        {
            var data = (await SelectAsync(p => p.StatusCode != (int)RecordStatusCode.Deleted)).ToList();

            var toReturn = data.Select(p => Mapper.ToModel<CatalogRequestAddressListItemModel>(p)).ToList();

            var countries = await _countryService.GetCountriesAsync(new CountryFilter());
            var states = countries.SelectMany(p => p.States).ToList();
            foreach(var item in toReturn)
            {
                var country = countries.FirstOrDefault(p => p.Id == item.IdCountry);
                item.Country = country?.CountryName;
                if(item.IdState!=0)
                {
                    var state = states.FirstOrDefault(p => p.Id == item.IdState);
                    item.StateCode = state?.StateCode;
                }
            }

            return toReturn;
        }

        public async Task<bool> DeleteCatalogRequestsAsync()
        {
            var items = (await _catalogRequestAddressRepository.Query(p => p.StatusCode != (int)RecordStatusCode.Deleted).SelectAsync(false)).ToList();
            foreach(var item in items)
            {
                item.StatusCode = (int)RecordStatusCode.Deleted;
            }
            await _catalogRequestAddressRepository.UpdateRangeAsync(items);
            
            return true;
        }
    }
}