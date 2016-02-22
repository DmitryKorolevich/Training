using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services.Users;
using VC.Public.Models;
using VitalChoice.Interfaces.Services.Settings;
using Microsoft.AspNet.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using VitalChoice.Interfaces.Services;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.CatalogRequests;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VC.Public.Models.Help;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Core.Infrastructure.Helpers.ReCaptcha;
using System.Collections.Generic;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Business.Mail;
using VitalChoice.Ecommerce.Domain.Mail;
using System.IO;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Mvc.Abstractions;
using System.Net;
using VitalChoice.Infrastructure.Domain.Mail;

namespace VC.Public.Controllers
{
    [AllowAnonymous]
    public class HelpController : BaseMvcController
    {
        private const string RequestCatalogTempData = "request-catalog-messsage";
        private const string ContactServiceTempData = "contact-service-messsage";

        private readonly IExtendedDynamicServiceAsync<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> _catalogRequestAddressService;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly IDynamicMapper<AddressDynamic, CatalogRequestAddress> _catalogRequestAddressMapper;
        private readonly ReCaptchaValidator _reCaptchaValidator;
        private readonly IOptions<AppOptions> _options;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public HelpController(
            IExtendedDynamicServiceAsync<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> catalogRequestAddressService,
            IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<AddressDynamic, CatalogRequestAddress> catalogRequestAddressMapper,
            ReCaptchaValidator reCaptchaValidator,
            IOptions<AppOptions> options,
            INotificationService notificationService,
            ILoggerProviderExtended loggerProvider)
        {
            _catalogRequestAddressService = catalogRequestAddressService;
            _appInfrastructureService = appInfrastructureService;
            _catalogRequestAddressMapper = catalogRequestAddressMapper;
            _reCaptchaValidator = reCaptchaValidator;
            _options = options;
            _notificationService = notificationService;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpGet]
        public Task<IActionResult> Error(int id)
        {
            if((HttpStatusCode)id==HttpStatusCode.NotFound)
            {
                return Task.FromResult<IActionResult>(Redirect("/content/"+ ContentConstants.NOT_FOUND_PAGE_URL));
            }
            else
            {
                return Task.FromResult<IActionResult>(Redirect("~/Shared/Error"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RequestCatalog(CatalogRequestAddressModel model)
        {
            if (!Validate(model))
            {
                return PartialView("_RequestCatalog", model);
            }
            if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
            {
                ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
                return PartialView("_RequestCatalog", model);
            }

            var address = await _catalogRequestAddressMapper.FromModelAsync(model, (int)AddressType.CatalogRequest);
            address = await _catalogRequestAddressService.InsertAsync(address);
            //TODO: - add sign up for newsletter(SignUpNewsletter)
            ViewBag.SuccessMessage= InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];
            ModelState.Clear();
            return PartialView("_RequestCatalog");
        }

        [HttpPost]
        public async Task<IActionResult> ContactCustomerService(CustomerServiceRequestModel model)
        {
            if (!Validate(model))
            {
                return PartialView("_ContactCustomerService", model);
            }
            if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
            {
                ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
                return PartialView("_ContactCustomerService", model);
            }

            CustomerServiceEmail emailData = new CustomerServiceEmail();
            emailData.Name = model.Name;
            emailData.Email = model.Email;
            emailData.Comment = model.Comment;
            var toEmail = model.Type == CustomerServiceRequestType.CustomerService ? _options.Value.CustomerServiceToEmail :
                _options.Value.CustomerFeedbackToEmail;
            await _notificationService.SendCustomerServiceEmailAsync(toEmail, emailData);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];
            ModelState.Clear();
            return PartialView("_ContactCustomerService");
        }

        [HttpPost]
        public async Task<IActionResult> PrivacyRequest(PrivacyRequestModel model)
        {
            if (!Validate(model))
            {
                return PartialView("_PrivacyRequestForm", model);
            }
            if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
            {
                ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
                return PartialView("_PrivacyRequestForm", model);
            }

            PrivacyRequestEmail emailData = new PrivacyRequestEmail();
            emailData.Name = model.Name;
            emailData.MailingAddress = model.MailingAddress;
            emailData.OtherName = model.OtherName;
            emailData.OtherAddress = model.OtherAddress;
            emailData.Comment = model.Comment;
            await _notificationService.SendPrivacyRequestEmailAsync(_options.Value.CustomerServiceToEmail, emailData);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];
            ModelState.Clear();
            return PartialView("_PrivacyRequestForm");
        }       


        [HttpGet]
        public async Task<IActionResult> SendContentUrlNotification(string name, string url, int type= (int)SendContentUrlType.Article)
        {
            ViewBag.Url = url;
            ViewBag.Name = name;

            return PartialView("_SendContentUrlNotification", new SendContentUrlNotificationModel {
                Url = url,
                Name=name,
                Type=(SendContentUrlType)type,
            });
        }

        [HttpPost]
        public async Task<IActionResult> SendContentUrlNotification(SendContentUrlNotificationModel model)
        {
            if (Validate(model))
            {
                if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
                {
                    ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
                }
                else
                {
                    ContentUrlNotificationEmail emailModel = new ContentUrlNotificationEmail();
                    emailModel.FromEmail = model.YourEmail;
                    emailModel.FromName = model.YourName;
                    emailModel.RecipentName = model.RecipentName;
                    emailModel.Url = model.Url;
                    emailModel.Name = model.Name;
                    emailModel.Message = model.Message;
                    if (model.Type == SendContentUrlType.Article)
                    {
                        await _notificationService.SendContentUrlNotificationForArticleAsync(model.RecipentEmail, emailModel);
                    }
                    else
                    {
                        await _notificationService.SendContentUrlNotificationForRecipeAsync(model.RecipentEmail, emailModel);
                    }

                    ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];
                    return PartialView("_SendContentUrlNotificationInner", model);
                }
            }

            return PartialView("_SendContentUrlNotificationInner", model);
        }
    }
}