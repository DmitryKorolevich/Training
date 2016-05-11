﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using System.Text;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using Microsoft.Extensions.OptionsModel;
using VC.Admin.Models.Affiliates;
using VitalChoice.Infrastructure.Domain.Options;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Affiliates)]
    public class AffiliateController : BaseApiController
    {
        private readonly IAffiliateService _affiliateService;
        private readonly IDynamicMapper<AffiliateDynamic, Affiliate> _mapper;
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly Country _defaultCountry;
        private readonly ICsvExportService<AffiliateOrderListItemModel, AffiliateOrderListItemModelCsvMap> _csvExportAffiliateOrderListItemService;
        private readonly IOrderService _orderService;
        private readonly ICountryService _countryService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly IOptions<AppOptions> _appOptions;
        private readonly ILogger logger;

        public AffiliateController(
            IAffiliateService affiliateService,
            IAffiliateUserService affiliateUserService,
            ILoggerProviderExtended loggerProvider,
            IDynamicMapper<AffiliateDynamic, Affiliate> mapper,
            IAppInfrastructureService appInfrastructureService,
            ICsvExportService<AffiliateOrderListItemModel, AffiliateOrderListItemModelCsvMap> csvExportAffiliateOrderListItemService,
            IOrderService orderService,
            ICountryService countryService,
            IOptions<AppOptions> appOptions,
            IObjectHistoryLogService objectHistoryLogService)
        {
            _affiliateService = affiliateService;
            _affiliateUserService = affiliateUserService;
            _mapper = mapper;
            _objectHistoryLogService = objectHistoryLogService;
            _defaultCountry = appInfrastructureService.Data().DefaultCountry;
            _csvExportAffiliateOrderListItemService = csvExportAffiliateOrderListItemService;
            _orderService = orderService;
            _countryService = countryService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            _appOptions = appOptions;
            logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpPost]
        public async Task<Result<PagedList<VCustomerInAffiliate>>> GetCustomerInAffiliateReport([FromBody]FilterBase filter)
        {
            var result = await _affiliateService.GetCustomerInAffiliateReport(filter);

            var toReturn = new PagedList<VCustomerInAffiliate>
            {
                Items = result.Items,
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<PagedList<AffiliateListItemModel>>> GetAffiliates([FromBody]VAffiliateFilter filter)
        {
            var result = await _affiliateService.GetAffiliatesAsync(filter);

            var toReturn = new PagedList<AffiliateListItemModel>
            {
                Items = result.Items.Select(p => new AffiliateListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<AffiliateManageModel>> GetAffiliate(int id)
        {
            if (id == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new AffiliateManageModel()
                {
                    StatusCode = RecordStatusCode.NotActive,
                    IdCountry = _defaultCountry != null ? _defaultCountry.Id : (int?)null,
                    PaymentType = AffiliateConstants.DefaultPaymentType,//Credit
                    Tier = AffiliateConstants.DefaultTier,
                    CommissionFirst = AffiliateConstants.DefaultCommissionFirst,
                    CommissionAll = AffiliateConstants.DefaultCommissionAll,
                };
            }

            var item = await _affiliateService.SelectAsync(id);
            if (item == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }
            AffiliateManageModel toReturn = await _mapper.ToModelAsync<AffiliateManageModel>(item);

            var login = await _affiliateUserService.GetAsync(toReturn.Id);
            if (login == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
            }

            toReturn.IsConfirmed = login.IsConfirmed;
            toReturn.PublicUserId = login.PublicId;

            CountryFilter filter = new CountryFilter();
            var countries = await _countryService.GetCountriesAsync(filter);
            var country = countries.FirstOrDefault(p => p.Id == toReturn.IdCountry);
            if(country!=null)
            {
                toReturn.CountryCode = country.CountryCode;
                var state = country.States.FirstOrDefault(p => p.Id == toReturn.IdState);
                if(state!=null)
                {
                    toReturn.StateCode = state.StateCode;
                }
            }

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<AffiliateManageModel>> UpdateAffiliate([FromBody]AffiliateManageModel model)
        {
            if (!Validate(model))
                return null;
            var affiliate = await _mapper.FromModelAsync(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                affiliate.IdEditedBy = userId;
            }
            if (affiliate.Id > 0)
            {
                affiliate = await _affiliateService.UpdateAsync(affiliate);
            }
            else
            {
                affiliate = await _affiliateService.InsertAsync(affiliate);
            }

            var toReturn = await _mapper.ToModelAsync<AffiliateManageModel>(affiliate);

            toReturn.IsConfirmed = model.IsConfirmed;
            toReturn.PublicUserId = model.PublicUserId;

            return toReturn;
        }

        [HttpGet]
        public Task<Result<AffiliateEmailModel>> GetAffiliateEmail(int id)
        {
            AffiliateEmailModel toReturn = new AffiliateEmailModel();
            toReturn.FromName = "Vital Choice";
            toReturn.FromEmail = "affiliatesupport@vitalchoice.com";
            toReturn.Subject = "Your Vital Choice affiliate account is ready.";
            if (id == 1)//notify
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Dear {0},");
                builder.AppendLine();
                builder.AppendLine("Your Vital Choice Affiliate account is ready for use. Your User Name is your e-mail address ({1}).  You can log into your account at https://www.vitalchoice.com/shop/pc/AffiliateLogin.asp to retrieve special links of Vital Choice text and banner ads that contain your affiliate ID.");
                builder.AppendLine();
                builder.AppendLine("Program overview: http://www.vitalchoice.com/shop/pc/viewContent.asp?idpage=22");
                builder.AppendLine();
                builder.AppendLine("We appreciate your advocacy and interest in promoting wild seafood nutrition. Looking forward to working together.");
                builder.AppendLine();
                builder.AppendLine("Questions or comments? Contact us at affiliatesupport@vitalchoice.com");
                builder.AppendLine();
                builder.AppendLine("Kind regards,");
                builder.AppendLine();
                builder.AppendLine("The Vital Choice Team");
                toReturn.Message = builder.ToString();
            }
            else if (id == 2)//email
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Dear {0},");
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine("Questions or comments? Contact us at affiliatesupport@vitalchoice.com");
                builder.AppendLine();
                builder.AppendLine("Kind regards,");
                builder.AppendLine();
                builder.AppendLine("The Vital Choice Team");
                toReturn.Message = builder.ToString();
            }

            return Task.FromResult<Result<AffiliateEmailModel>>(toReturn);
        }

        [HttpPost]
        public async Task<Result<bool>> SendAffiliateEmail([FromBody]AffiliateEmailModel model)
        {
            if (!Validate(model))
                return false;
            var item = model.Convert();
            return await _affiliateService.SendAffiliateEmailAsync(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteAffiliate(int id, [FromBody] object model)
        {
            return await _affiliateService.DeleteAsync(id);
        }

        [HttpPost]
        public async Task<Result<bool>> ResendActivation(Guid id, [FromBody] object model)
        {
            await _affiliateUserService.ResendActivationAsync(id);

            return true;
        }

        [HttpPost]
        public async Task<Result<bool>> ResetPassword(Guid id, [FromBody] object model)
        {
            await _affiliateUserService.SendResetPasswordAsync(id);

            return true;
        }

        [HttpGet]
        public async Task<ActionResult> LoginAsAffiliate(int id)
        {
            var token = await _affiliateUserService.GenerateLoginTokenAsync(id);
            var url = $"https://{_appOptions.Value.PublicHost}/AffiliateAccount/LoginAsAffiliate/{token}";

            return Redirect(url);
        }

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (AffiliateDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(AffiliateDynamic));
                var model = await _mapper.ToModelAsync<AffiliateManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (AffiliateDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(AffiliateDynamic));
                var model = await _mapper.ToModelAsync<AffiliateManageModel>(dynamic);
                toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<PagedList<AffiliateOrderListItemModel>>> GetAffiliateOrderPaymentsWithCustomerInfo([FromBody]AffiliateOrderPaymentFilter filter)
        {
            return await _orderService.GetAffiliateOrderPaymentsWithCustomerInfo(filter);
        }

        [HttpGet]
        public async Task<Result<ICollection<OrderPaymentListItemModel>>> GetUnpaidOrdersForLastPeriod(int id)
        {
            AffiliateOrderPaymentFilter filter = new AffiliateOrderPaymentFilter();
            filter.IdAffiliate = id;
            filter.Status = AffiliateOrderPaymentStatus.NotPaid;
            filter.To = GetLastMonthStartDayFromPSTInLocal();
            var result = await _affiliateService.GetAffiliateOrderPayments(filter);
            var toReturn = result.Items.Select(p => new OrderPaymentListItemModel(p)).ToList();
            return toReturn;
        }

        [HttpGet]
        public async Task<Result<AffiliatesSummaryModel>> GetAffiliatesSummary()
        {
            return await _affiliateService.GetAffiliatesSummary();
        }

        [HttpPost]
        public async Task<Result<bool>> PayForAffiliateOrders(int id, [FromBody] object model)
        {
            return await _affiliateService.PayForAffiliateOrders(id, GetLastMonthStartDayFromPSTInLocal());
        }

        [NonAction]
        private DateTime GetLastMonthStartDayFromPSTInLocal()
        {
            DateTime lastMonthStartDay = DateTime.Now;
            lastMonthStartDay = new DateTime(lastMonthStartDay.Year, lastMonthStartDay.Month, 1);
            lastMonthStartDay = TimeZoneInfo.ConvertTime(lastMonthStartDay, _pstTimeZoneInfo, TimeZoneInfo.Local);
            return lastMonthStartDay;
        }

        [HttpGet]
        public async Task<Result<ICollection<AffiliatesSummaryReportItemModel>>> GetAffiliatesSummaryReportItemsForMonths(int count = 3, bool include = true)
        {
            if (count > 12)
                count = 12;
            if (count < 3)
                count = 3;
            if (include)
                count++;
            DateTime lastMonthStartDay = DateTime.Now;
            lastMonthStartDay = new DateTime(lastMonthStartDay.Year, lastMonthStartDay.Month, 1);
            if (!include)
            {
                lastMonthStartDay = lastMonthStartDay.AddMonths(-1);
            }
            lastMonthStartDay = TimeZoneInfo.ConvertTime(lastMonthStartDay, _pstTimeZoneInfo, TimeZoneInfo.Local);
            return (await _affiliateService.GetAffiliatesSummaryReportItemsForMonths(lastMonthStartDay, count)).ToList();
        }

        [HttpGet]
        public async Task<Result<ICollection<PaymentHistoryLineItemModel>>> GetAffiliatePaymentHistory(int id)
        {
            return (await _affiliateService.GetAffiliatePayments(id)).Select(p => new PaymentHistoryLineItemModel()
            {
                DateCreated = p.DateCreated,
                Amount = p.Amount,
            }).ToList();
        }
    }
}