using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System.Collections.Generic;
using VC.Admin.Models.Setting;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.CatalogRequests;
using Microsoft.Net.Http.Headers;
using System;
using Newtonsoft.Json;
using VC.Admin.Models.ContentManagement;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Data.Extensions;

namespace VC.Admin.Controllers
{
    public class SettingController : BaseApiController
    {
        private readonly ILogViewService logViewService;
        private readonly ICountryService countryService;
        private readonly ISettingService settingService;
        private readonly IFileService fileService;
        private readonly IObjectHistoryLogService objectHistoryLogService;
        private readonly ICatalogRequestAddressService _catalogRequestAddressService;
        private readonly ICsvExportService<CatalogRequestAddressListItemModel, CatalogRequestAddressListItemModelCsvMap> _exportCatalogRequestAddressService;
        private readonly ILogger logger;

        public SettingController(
            ILogViewService logViewService,
            ICountryService countryService,
            ISettingService settingService,
            IFileService fileService,
            IObjectHistoryLogService objectHistoryLogService,
            ICatalogRequestAddressService catalogRequestAddressService,
            ICsvExportService<CatalogRequestAddressListItemModel, CatalogRequestAddressListItemModelCsvMap> exportCatalogRequestAddressService,
            ILoggerProviderExtended loggerProvider)
        {
            this.logViewService = logViewService;
            this.countryService = countryService;
            this.settingService = settingService;
            this.fileService = fileService;
            this.objectHistoryLogService = objectHistoryLogService;
            _catalogRequestAddressService = catalogRequestAddressService;
            _exportCatalogRequestAddressService = exportCatalogRequestAddressService;
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        #region Lookups

        [HttpGet]
        public async Task<Result<IList<Lookup>>> GetLookups()
        {
            var result = await settingService.GetLookupsAsync(SettingConstants.SETTINGS_LOOKUP_NAMES.Split(','));
            result.ForEach(p =>
            {
                if (p.LookupVariants != null)
                {
                    foreach (var lookupVariant in p.LookupVariants)
                    {
                        lookupVariant.Lookup = null;
                    }
                }
            });

            return result.ToList();
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<Lookup>> GetLookup(int id)
        {
            var result = await settingService.GetLookupAsync(id);
            if (result.LookupVariants != null)
            {
                foreach (var lookupVariant in result.LookupVariants)
                {
                    lookupVariant.Lookup = null;
                }
            }

            return result;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<bool>> UpdateLookupVariants(int id, [FromBody]ICollection<LookupVariant> model)
        {
            var result = await settingService.UpdateLookupVariantsAsync(id, model);

            return result;
        }

        #endregion

        #region Countries/States

        [HttpPost]
        public async Task<Result<IEnumerable<CountryListItemModel>>> GetCountries([FromBody] object model)
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
        public async Task<Result<bool>> DeleteCountry(int id, [FromBody] object model)
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
        public async Task<Result<bool>> DeleteState(int id, [FromBody] object model)
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
            var items = await _catalogRequestAddressService.GetCatalogRequestsAsync();

            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }
            var result = await logViewService.GetCommonItemsAsync(filter.LogLevel, filter.Message, filter.Source, filter.From, filter.To?.AddDays(1),
                filter.Paging.PageIndex, filter.Paging.PageItemCount, filter.Sorting);
            var toReturn = new PagedList<LogListItemModel>
            {
                Items = result.Items.Select(p => new LogListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        #endregion

        #region ObjectHistoryLogs

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn != null)
            {
                var settings = GetObjectHistoryLogTypesForObjectType(filter.IdObjectType);
                if (toReturn.Main != null && !string.IsNullOrEmpty(toReturn.Main.Data))
                {
                    var contentItem = Convert.ChangeType(JsonConvert.DeserializeObject(toReturn.Main.Data, settings.Key), settings.Key);
                    var model = Activator.CreateInstance(settings.Value, contentItem);
                    toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                    });
                }
                if (toReturn.Before != null && !string.IsNullOrEmpty(toReturn.Before.Data))
                {
                    var contentItem = Convert.ChangeType(JsonConvert.DeserializeObject(toReturn.Before.Data, settings.Key), settings.Key);
                    var model = Activator.CreateInstance(settings.Value, contentItem);
                    toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                    });
                }
            }

            return toReturn;
        }

        private KeyValuePair<Type, Type> GetObjectHistoryLogTypesForObjectType(ObjectType idObjectType)
        {
            KeyValuePair<Type, Type> toReturn;
            switch (idObjectType)
            {
                case ObjectType.Article:
                    toReturn = new KeyValuePair<Type, Type>(typeof (Article), typeof (ArticleManageModel));
                    break;
                case ObjectType.FAQ:
                    toReturn = new KeyValuePair<Type, Type>(typeof(FAQ), typeof(FAQManageModel));
                    break;
                case ObjectType.Recipe:
                    toReturn = new KeyValuePair<Type, Type>(typeof(Recipe), typeof(RecipeManageModel));
                    break;
                case ObjectType.ContentPage:
                    toReturn = new KeyValuePair<Type, Type>(typeof(ContentPage), typeof(ContentPageManageModel));
                    break;
                case ObjectType.ContentCategory:
                    toReturn = new KeyValuePair<Type, Type>(typeof(ContentCategory), typeof(CategoryManageModel));
                    break;
                case ObjectType.ProductCategoryContent:
                    toReturn = new KeyValuePair<Type, Type>(typeof(ProductCategoryContent), typeof(ProductCategoryManageModel));
                    break;
                default:
                    throw new NotImplementedException();
            }
            return toReturn;
        }

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

        [HttpPost]
        public async Task<Result<PagedList<ObjectHistoryOrderLogListItemModel>>> GetOrderObjectHistoryLogItems([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var result = await objectHistoryLogService.GetObjectHistoryLogItems(filter);
            var toReturn = new PagedList<ObjectHistoryOrderLogListItemModel>
            {
                Items = result.Items.Select(p => new ObjectHistoryOrderLogListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        #endregion

        #region RequestCatalogs

        [HttpGet]
        public async Task<Result<ICollection<CatalogRequestAddressListItemModel>>> GetCatalogRequests()
        {
            var items = await _catalogRequestAddressService.GetCatalogRequestsAsync();

            return items.ToList();
        }

        [HttpGet]
        public async Task<FileResult> GetCatalogRequestsReportFile()
        {
            var items = await _catalogRequestAddressService.GetCatalogRequestsAsync();
            var data = _exportCatalogRequestAddressService.ExportToCsv(items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.CATALOG_REQUESTS_REPORT, DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss")),
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteCatalogRequests([FromBody] object model)
        {
            var toReturn = await _catalogRequestAddressService.DeleteCatalogRequestsAsync();

            return toReturn;
        }

        #endregion
    }
}