using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models;
using VC.Admin.Models.Account;
using VC.Admin.Models.Customers;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VC.Admin.Models.Public;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Mailings;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    [AllowAnonymous]
    public class PublicController : BaseApiController
    {
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ReferenceData _referenceData;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public PublicController(
            ISettingService settingService,
            INotificationService notificationServic,
            ReferenceData referenceData,
            ICountryNameCodeResolver countryNameCodeResolver)
        {
            _notificationService = notificationServic;
            _settingService = settingService;
            _referenceData = referenceData;
            _countryNameCodeResolver = countryNameCodeResolver;
        }

        [HttpGet]
        public async Task<Result<ICollection<LookupViewModel>>> GetEmailOrderSettings()
        {
            var items = await _settingService.GetLookupsAsync(SettingConstants.EMAIL_ORDER_LOOKUP_NAMES.Split(','));

            return items.Select(p => new LookupViewModel(p.Name, null, null, p)).ToList();
        }

        [HttpGet]
        public async Task<Result<EmailOrderManageModel>> GetEmailOrder()
        {
            var defaultCountry = _referenceData.DefaultCountry;
            var toReturn = new EmailOrderManageModel()
            {
                Shipping = new AddressModel() { Country = new CountryListItemModel(defaultCountry) },
                SkuOrdereds = new List<SkuOrderedManageModel>() { new SkuOrderedManageModel(null) },
                IdPaymentMethodType = (int)PaymentMethodType.Marketing
            };

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> SendEmailOrder([FromBody] EmailOrderManageModel model)
        {
            if (!Validate(model))
                return false;

            var lookups = await _settingService.GetLookupsAsync(SettingConstants.EMAIL_ORDER_LOOKUP_NAMES.Split(','));
            var requestorsLookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.EMAIL_ORDER_REQUESTOR_LOOKUP_NAME);
            var reasonsLookup = lookups.FirstOrDefault(p => p.Name == SettingConstants.EMAIL_ORDER_REASON_LOOKUP_NAME);

            var email = new EmailOrderEmail()
            {
                DateCreated = DateTime.Now,
                DetailsOnEvent = model.DetailsOnEvent,
                Instuction = model.Instuction,
                PaymentMethodType = model.IdPaymentMethodType.HasValue ?
                                LookupHelper.GetShortPaymentMethodName((PaymentMethodType)model.IdPaymentMethodType.Value)
                                : null,
                Requestor = model.IdRequestor.HasValue ?
                            requestorsLookup.LookupVariants.FirstOrDefault(p=>p.Id==model.IdRequestor.Value)?.ValueVariant
                            : null,
                Reason = model.IdReason.HasValue ?
                            reasonsLookup.LookupVariants.FirstOrDefault(p => p.Id == model.IdReason.Value)?.ValueVariant
                            : null,
                Shipping = new AddressBaseModel()
                {
                    Company = model.Shipping.Company,
                    FirstName = model.Shipping.FirstName,
                    LastName = model.Shipping.LastName,
                    Address1 = model.Shipping.Address1,
                    Address2 = model.Shipping.Address2,
                    City = model.Shipping.City,
                    County = model.Shipping.County,
                    Country = model.Shipping.Country!=null ? _countryNameCodeResolver.GetCountryName(model.Shipping.Country.Id) : null,
                    State = model.Shipping.Country!=null ?_countryNameCodeResolver.GetStateName(model.Shipping.Country.Id, model.Shipping.State) : null,
                    Zip = model.Shipping.Zip,
                    Fax = model.Shipping.Fax,
                    Phone = model.Shipping.Phone,
                }
            };
            email.DateCreatedDatePart = email.DateCreated.ToString("MM/dd/yyyy");
            email.DateCreatedTimePart = email.DateCreated.ToString("hh:mm tt");
            if (model.IdPaymentMethodType == (int)PaymentMethodType.Marketing && model.Marketing != null)
            {
                email.MarketingPromotionType = model.Marketing.MarketingPromotionType.HasValue
                    ? _referenceData.MarketingPromotionTypesNotHidden.FirstOrDefault(
                        p => p.Key == model.Marketing.MarketingPromotionType.Value)?.Text
                    : null;
                email.PaymentComment = model.Marketing.PaymentComment;
            }
            if (model.IdPaymentMethodType == (int)PaymentMethodType.NoCharge && model.NC != null)
            {
                email.PaymentComment = model.NC.PaymentComment;
            }
            email.Skus = model.SkuOrdereds?.Where(p=>!string.IsNullOrEmpty(p.Code) && p.QTY.HasValue && p.Price.HasValue).Select(p => new EmailOrderSku()
                         {
                             Code = p.Code,
                             QTY = p.QTY.Value,
                             Price = p.Price.Value
                         }).ToList() ?? new List<EmailOrderSku>();
                        

            await _notificationService.SendEmailOrderEmailAsync(email);

            return true;
        }
    }
}