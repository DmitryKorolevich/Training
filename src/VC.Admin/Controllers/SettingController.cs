using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using System.Collections.Generic;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Services;
using VitalChoice.Core.Base;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Constants;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Domain.Transfer.Settings;
using Newtonsoft.Json;
using Microsoft.Dnx.Runtime.Infrastructure;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce;
using System;
using VitalChoice.DynamicData.Entities;
using VC.Admin.Models.Order;
using Autofac;
using Microsoft.Framework.DependencyInjection;

namespace VC.Admin.Controllers
{
    public class SettingController : BaseApiController
    {
        private readonly ILogViewService logViewService;
        private readonly ICountryService countryService;
        private readonly ISettingService settingService;
        private readonly IFileService fileService;
        private readonly IObjectHistoryLogService objectHistoryLogService;
        private readonly ILogger logger;

        public SettingController(
            ILogViewService logViewService,
            ICountryService countryService, 
            ISettingService settingService,
            IFileService fileService,
            IObjectHistoryLogService objectHistoryLogService,
            ILoggerProviderExtended loggerProvider)
        {
            this.logViewService = logViewService;
            this.countryService = countryService;
            this.settingService = settingService;
            this.fileService = fileService;
            this.objectHistoryLogService = objectHistoryLogService;
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        #region Countries/States

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<IEnumerable<CountryListItemModel>>> GetCountries()
        {
            var result = await countryService.GetCountriesAsync();

            return result.Select(p => new CountryListItemModel(p)).ToList();
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<bool>> UpdateCountriesOrder([FromBody]IEnumerable<CountryListItemModel> model)
        {
            List<Country> countries = new List<Country>();
            foreach (var item in model)
            {
                countries.Add(item.Convert());
            }

            return await countryService.UpdateCountriesOrderAsync(countries);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<CountryManageModel>> UpdateCountry([FromBody]CountryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await countryService.UpdateCountryAsync(item);

            return new CountryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<bool>> DeleteCountry(int id)
        {
            return await countryService.DeleteCountryAsync(id);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<StateManageModel>> UpdateState([FromBody]StateManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await countryService.UpdateStateAsync(item);

            return new StateManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<bool>> DeleteState(int id)
        {
            return await countryService.DeleteStateAsync(id);
        }

        #endregion

        #region Settings

        [HttpGet]
        [SuperAdminAuthorize()]
        public async Task<Result<GlobalSettingsManageModel>> GetGlobalSettings()
        {
            var items = await settingService.GetAppSettingItemsAsync(new List<string>
            {
                SettingConstants.GLOBAL_PERISHABLE_THRESHOLD_NAME,
                SettingConstants.GLOBAL_CREDIT_CARD_AUTHORIZATIONS_NAME,
            });

            return new GlobalSettingsManageModel(items);
        }

        [HttpPost]
        [SuperAdminAuthorize()]
        public async Task<Result<GlobalSettingsManageModel>> UpdateGlobalSettings([FromBody]GlobalSettingsManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();

            var appSettingItems = await settingService.UpdateAppSettingItemsAsync(item);

            return new GlobalSettingsManageModel(appSettingItems);
        }

        #endregion

        #region Logs

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<PagedList<LogListItemModel>>> GetLogItems([FromBody]LogItemListFilter filter)
        {
            var result = await logViewService.GetCommonItemsAsync(filter.LogLevel,filter.Message, filter.Source, filter.From, filter.To?.AddDays(1),
                filter.Paging.PageIndex, filter.Paging.PageItemCount, filter.Sorting);
            var toReturn = new PagedList<LogListItemModel>
            {
                Items = result.Items.Select(p=>new LogListItemModel(p)).ToList(),
                Count= result.Count,
            };

            return toReturn;
        }

        #endregion

        #region ObjectHistoryLogs

        [HttpPost]
        public async Task<Result<PagedList<ObjectHistoryLogListItemModel>>> GetObjectHistoryLogItems([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var result = await objectHistoryLogService.GetObjectHistoryLogItems(filter);
            var toReturn = new PagedList<ObjectHistoryLogListItemModel>
            {
                Items = result.Items.Select(p => new ObjectHistoryLogListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        #endregion
    }
}