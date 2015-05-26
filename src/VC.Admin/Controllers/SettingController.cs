using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using System.Collections.Generic;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Constants;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Base;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Settings)]
    public class SettingController : BaseApiController
    {
        private readonly ILogViewService logViewService;
        private readonly ICountryService countryService;
        private readonly ISettingService settingService;
        private readonly IFileService fileService;
        private readonly ILogger logger;

        public SettingController(ILogViewService logViewService, ICountryService countryService, ISettingService settingService, IFileService fileService)
        {
            this.logViewService = logViewService;
            this.countryService = countryService;
            this.settingService = settingService;
            this.fileService = fileService;
            this.logger = LoggerService.GetDefault();
        }

        #region Countries/States

        [HttpPost]
        public async Task<Result<IEnumerable<CountryListItemModel>>> GetCountries()
        {
            var result = await countryService.GetCountriesAsync();

            return result.Select(p => new CountryListItemModel(p)).ToList();
        }

        [HttpPost]
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
        public async Task<Result<CountryManageModel>> UpdateCountry([FromBody]CountryManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await countryService.UpdateCountryAsync(item);

            return new CountryManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteCountry(int id)
        {
            return await countryService.DeleteCountryAsync(id);
        }

        [HttpPost]
        public async Task<Result<StateManageModel>> UpdateState([FromBody]StateManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await countryService.UpdateStateAsync(item);

            return new StateManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteState(int id)
        {
            return await countryService.DeleteStateAsync(id);
        }

        #endregion

        #region Settings

        [HttpGet]
        public async Task<Result<string>> GetGlobalPerishableThreshold()
        {
            string toReturn = null;
            var result = await settingService.GetAppSettingAsync(SettingConstants.GLOBAL_PERISHABLE_THRESHOLD);
            if(result!=null)
            {
                toReturn = result.Value;
            }

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<string>> UpdateGlobalPerishableThreshold(string id)
        {
            string toReturn = null;
            var result = await settingService.UpdateAppSettingAsync(SettingConstants.GLOBAL_PERISHABLE_THRESHOLD,id?.Trim());
            if (result != null)
            {
                toReturn = result.Value;
            }

            return toReturn;
        }

        #endregion

        #region Logs

        [HttpPost]
        public async Task<Result<PagedList<LogListItemModel>>> GetLogItems([FromBody]LogItemListFilter filter)
        {
            var result = await logViewService.GetCommonItemsAsync(filter.LogLevel,filter.Message, filter.Source, filter.From, filter.To?.AddDays(1),
                filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var toReturn = new PagedList<LogListItemModel>
            {
                Items = result.Items.Select(p=>new LogListItemModel(p)).ToList(),
                Count= result.Count,
            };

            return toReturn;
        }

        #endregion
    }
}