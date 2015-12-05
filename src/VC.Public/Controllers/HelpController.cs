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
        public Task<IActionResult> RequestCatalog()
        {
            if (TempData.ContainsKey(RequestCatalogTempData))
            {
                ViewBag.SuccessMessage = TempData[RequestCatalogTempData];
            }
            return Task.FromResult<IActionResult>(View(GetCatalogRequestAddressPrototype()));
        }

        [NonAction]
        private CatalogRequestAddressModel GetCatalogRequestAddressPrototype()
        {
            CatalogRequestAddressModel model = new CatalogRequestAddressModel();
            if (_appInfrastructureService.Get().DefaultCountry != null)
            {
                model.IdCountry = _appInfrastructureService.Get().DefaultCountry.Id;
            }
            return model;
        }

        [HttpPost]
        public async Task<IActionResult> RequestCatalog(CatalogRequestAddressModel model)
        {
            if (!Validate(model))
            {
                return View(model);
            }
            if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
            {
                ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
                return View(model);
            }

            var address = _catalogRequestAddressMapper.FromModel(model);
            address.IdObjectType = (int)AddressType.Shipping;
            address = await _catalogRequestAddressService.InsertAsync(address);
            //TODO: - add sign up for newsletter(SignUpNewsletter)
            TempData[RequestCatalogTempData] = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];
            return RedirectToAction("RequestCatalog");
        }

        [HttpGet]
        public Task<IActionResult> ContactCustomerService()
        {
            if (TempData.ContainsKey(ContactServiceTempData))
            {
                ViewBag.SuccessMessage = TempData[ContactServiceTempData];
            }

            CustomerServiceRequestModel model = new CustomerServiceRequestModel();

            return Task.FromResult<IActionResult>(View(model));
        }

        [HttpPost]
        public async Task<IActionResult> ContactCustomerService(CustomerServiceRequestModel model)
        {
            if (!Validate(model))
            {
                return View(model);
            }
            if (!await _reCaptchaValidator.Validate(Request.Form[ReCaptchaValidator.DefaultPostParamName]))
            {
                ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.WrongCaptcha]);
                return View(model);
            }

            CustomerServiceEmail emailData = new CustomerServiceEmail();
            emailData.Name = model.Name;
            emailData.Email = model.Email;
            emailData.Comment = model.Comment;
            var toEmail = model.Type == CustomerServiceRequestType.CustomerService ? _options.Value.CustomerServiceToEmail :
                _options.Value.CustomerFeedbackToEmail;
            await _notificationService.SendCustomerServiceEmailAsync(toEmail, emailData);

            TempData[ContactServiceTempData] = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];

            return RedirectToAction("ContactCustomerService");
        }
    }
}