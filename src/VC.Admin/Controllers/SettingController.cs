using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using VC.Admin.Models.ContentManagement;
using VC.Admin.Models.EmailTemplates;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VC.Admin.Models.Settings;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Business.Services;
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
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.Emails;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Profiling.Base;

namespace VC.Admin.Controllers
{
    public class SettingController : BaseApiController
    {
        private const string Admin_Areas = "Admin Below Nav Critical Alert Message";
        private static readonly Regex TemplateReplaceExpression = new Regex("\\{([a-z_@][a-z0-9_\\.]*)\\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ILogViewService logViewService;
        private readonly ICountryService countryService;
        private readonly ISettingService settingService;
        private readonly IFileService fileService;
        private readonly IObjectHistoryLogService objectHistoryLogService;
        private readonly ICatalogRequestAddressService _catalogRequestAddressService;
        private readonly ICsvExportService<CatalogRequestAddressListItemModel, CatalogRequestAddressListItemModelCsvMap> _exportCatalogRequestAddressService;
        private readonly ILogger logger;
        private readonly ITableLogsClient _logsClient;
        private readonly AppSettings _appSettings;
        private readonly IContentAreaService _contentAreaService;

        public SettingController(
            ILogViewService logViewService,
            ICountryService countryService,
            ISettingService settingService,
            IFileService fileService,
            IObjectHistoryLogService objectHistoryLogService,
            ICatalogRequestAddressService catalogRequestAddressService,
            ICsvExportService<CatalogRequestAddressListItemModel, CatalogRequestAddressListItemModelCsvMap> exportCatalogRequestAddressService,
            ILoggerFactory loggerProvider, 
            ITableLogsClient logsClient,
            AppSettings appSettings,
            IContentAreaService contentAreaService)
        {
            this.logViewService = logViewService;
            this.countryService = countryService;
            this.settingService = settingService;
            this.fileService = fileService;
            this.objectHistoryLogService = objectHistoryLogService;
            _catalogRequestAddressService = catalogRequestAddressService;
            _exportCatalogRequestAddressService = exportCatalogRequestAddressService;
            _logsClient = logsClient;
            _appSettings = appSettings;
            this.logger = loggerProvider.CreateLogger<SettingController>();
            _contentAreaService = contentAreaService;
        }

        private static List<EditLockArea> _adminEditLockAreas;
        private const int secCount = 30;

        static SettingController()
        {
            _adminEditLockAreas= BaseAppConstants.ADMIN_EDIT_LOCK_AREAS != null ?
                        BaseAppConstants.ADMIN_EDIT_LOCK_AREAS.Split(',').
                        Select(p=> new EditLockArea(p)).ToList()
                        : new List<EditLockArea>();
        }

        #region EditLocks

        [HttpPost]
        public async Task<bool> EditLockPing([FromBody] EditLockPingModel model)
        {
            string browserUserAgent = Request.Headers["User-Agent"];
            var area = _adminEditLockAreas.FirstOrDefault(p => p.Name == model.AreaName);

            //not confugired
            if (area == null || model.Id == 0)
            {
                return true;
            }


            EditLockAreaItem currentStatus;
            lock (area.LockObject)
            {
                area.Items.TryGetValue(model.Id, out currentStatus);
            }

            if (currentStatus != null)
            {
                if (currentStatus.IdAgent == model.IdAgent && currentStatus.BrowserUserAgent == browserUserAgent)
                {
                    var now = DateTime.Now;
                    if (currentStatus.Expired <= now)
                    {
                        return false;
                    }
                    else
                    {
                        currentStatus.Expired = now.AddSeconds(30);
                    }
                }
            }

            return true;
        }

        [HttpPost]
        public async Task<EditLockRequestModel> EditLockRequest([FromBody]EditLockRequestModel model)
        {
            model.Avaliable = false;
            string browserUserAgent = Request.Headers["User-Agent"];

            var area = _adminEditLockAreas.FirstOrDefault(p => p.Name == model.AreaName);

            //not confugired - allow edit
            if (area == null || model.Id==0)
            {
                model.Avaliable = true;
                return model;
            }

            EditLockAreaItem currentStatus;
            lock (area.LockObject)
            {
                var now = new DateTime();
                if (!area.Items.TryGetValue(model.Id, out currentStatus))
                {
                    currentStatus = new EditLockAreaItem()
                    {
                        Agent = model.Agent,
                        IdAgent = model.IdAgent,
                        BrowserUserAgent = browserUserAgent,
                        Expired = now.AddSeconds(secCount)
                    };
                    area.Items.Add(model.Id, currentStatus);
                }

                if (currentStatus.Expired <= now)
                {
                    currentStatus.Agent = model.Agent;
                    currentStatus.IdAgent = model.IdAgent;
                    currentStatus.BrowserUserAgent = browserUserAgent;
                    currentStatus.Expired = now.AddSeconds(secCount);

                    model.Avaliable = true;
                    return model;
                }
                else
                {
                    //the same
                    if (currentStatus.IdAgent == model.IdAgent && currentStatus.BrowserUserAgent == browserUserAgent)
                    {
                        currentStatus.Expired = now.AddSeconds(secCount);

                        model.Avaliable = true;
                        return model;
                    }
                    else
                    {
                        //different
                        var toReturn= new EditLockRequestModel()
                        {
                            Agent = currentStatus.Agent,
                            IdAgent = currentStatus.IdAgent,
                            Avaliable = false,
                        };

                        return toReturn;
                    }
                }
            }
        }

        #endregion

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
        [AdminAuthorize(PermissionType.Settings)]
        public Result<GlobalSettingsManageModel> GetGlobalSettings()
        {
            return new GlobalSettingsManageModel()
            {
                GlobalPerishableThreshold = _appSettings.GlobalPerishableThreshold,
                CreditCardAuthorizations = _appSettings.CreditCardAuthorizations,
            };
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<bool>> UpdateGlobalSettings([FromBody] GlobalSettingsManageModel model)
        {
            if (!Validate(model))
                return false;

            var settings = settingService.GetSettings();
            settings.Data.GlobalPerishableThreshold = model.GlobalPerishableThreshold;
            settings.Data.CreditCardAuthorizations = model.CreditCardAuthorizations;

            return await settingService.UpdateSettingsAsync(settings);
        }

        [HttpGet]
        public async Task<Result<ICollection<ContentArea>>> GetContentAreas()
        {
            var names = Admin_Areas.Split(',');
            var areas = await _contentAreaService.GetContentAreaByNameAsync(names);

            return areas.ToList();
        }

        #endregion

        #region Logs

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public Result<PagedList<LogListItemModel>> GetLogItems([FromBody] LogItemListFilter filter)
        {
            var end = filter.To ?? DateTime.Today;
            var start = filter.From ?? DateTime.Today;
            var result = _logsClient.GetLogs(start, end, filter.AppName, filter.LogLevel, filter.Message, filter.Source,
                filter.Paging.PageIndex,
                filter.Paging.PageItemCount, filter.Sorting);
            var toReturn = new PagedList<LogListItemModel>
            {
                Items = result.Items.Select(p => new LogListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public Task<Result<PagedList<ProfileScopeListItemModel>>> GetProfileScopeItems([FromBody] FilterBase filter)
        {
            PagedList<ProfileScopeListItemModel> toReturn = new PagedList<ProfileScopeListItemModel>();
            var scopes = PerformanceRequestService.GetWorkedScopes();
            IEnumerable<ProfilingScope> scopesOrderedFiltered = scopes;
            toReturn.Count = scopes.Count;
            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                scopesOrderedFiltered =
                    scopesOrderedFiltered.Where(
                        s => (s.Data as string)?.Contains(filter.SearchText) ?? false);
            }
            if (!string.IsNullOrEmpty(filter.Sorting?.Path))
            {
                switch (filter.Sorting.Path)
                {
                    case "Start":
                        scopesOrderedFiltered = filter.Sorting.SortOrder == FilterSortOrder.Asc
                            ? scopesOrderedFiltered.OrderBy(s => s.Start)
                            : scopesOrderedFiltered.OrderByDescending(s => s.Start);
                        break;
                    case "TimeElapsed":
                        scopesOrderedFiltered = filter.Sorting.SortOrder == FilterSortOrder.Asc
                            ? scopesOrderedFiltered.OrderBy(s => s.TimeElapsed)
                            : scopesOrderedFiltered.OrderByDescending(s => s.TimeElapsed);
                        break;
                    case "ShortData":
                        scopesOrderedFiltered = filter.Sorting.SortOrder == FilterSortOrder.Asc
                            ? scopesOrderedFiltered.OrderBy(s => s.Data)
                            : scopesOrderedFiltered.OrderByDescending(s => s.Data);
                        break;
                }
            }
            toReturn.Items = scopesOrderedFiltered.Skip((filter.Paging.PageIndex - 1)*filter.Paging.PageItemCount).
                Take(filter.Paging.PageItemCount).Select(p => new ProfileScopeListItemModel(p)).ToList();

            return Task.FromResult<Result<PagedList<ProfileScopeListItemModel>>>(toReturn);
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
                case ObjectType.ContentArea:
                    toReturn = new KeyValuePair<Type, Type>(typeof(ContentArea), typeof(ContentAreaReadModel));
                    break;
                case ObjectType.MasterContentItem:
                    toReturn = new KeyValuePair<Type, Type>(typeof(MasterContentItem), typeof(MasterContentItemManageModel));
                    break;
                case ObjectType.EmailTemplate:
                    toReturn = new KeyValuePair<Type, Type>(typeof(EmailTemplate), typeof(EmailTemplateManageModel));
                    break;
                case ObjectType.CustomPublicStyle:
                    toReturn = new KeyValuePair<Type, Type>(typeof(CustomPublicStyle), typeof(StylesModel));
                    break;
                case ObjectType.GiftCertificate:
                    toReturn = new KeyValuePair<Type, Type>(typeof(GiftCertificate), typeof(GCManageModel));
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
        public async Task<Result<bool>> DeleteCatalogRequests()
        {
            var toReturn = await _catalogRequestAddressService.DeleteCatalogRequestsAsync();

            return toReturn;
        }

        #endregion
    }
}