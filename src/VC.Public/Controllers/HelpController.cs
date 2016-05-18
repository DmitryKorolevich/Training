﻿using System;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services.Users;
using VC.Public.Models;
using VitalChoice.Interfaces.Services.Settings;
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
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Business.Mail;
using VitalChoice.Ecommerce.Domain.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using VitalChoice.Business.Models.Help;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Validation.Models;

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
        private readonly IProductService _productService;
        private readonly BrontoService _brontoService;
        private readonly ILogger _logger;

        public HelpController(
            IExtendedDynamicServiceAsync<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> catalogRequestAddressService,
            IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<AddressDynamic, CatalogRequestAddress> catalogRequestAddressMapper,
            ReCaptchaValidator reCaptchaValidator,
            IOptions<AppOptions> options,
            INotificationService notificationService,
            ILoggerProviderExtended loggerProvider,
            IProductService productService,
            BrontoService brontoService,
            IPageResultService pageResultService) : base(pageResultService)
        {
            _catalogRequestAddressService = catalogRequestAddressService;
            _appInfrastructureService = appInfrastructureService;
            _catalogRequestAddressMapper = catalogRequestAddressMapper;
            _reCaptchaValidator = reCaptchaValidator;
            _options = options;
            _notificationService = notificationService;
            _productService = productService;
            _brontoService = brontoService;
            _logger = loggerProvider.CreateLogger<HelpController>();
        }

        [HttpGet]
        public Task<IActionResult> Error(int id)
        {
            if((HttpStatusCode)id==HttpStatusCode.NotFound)
            {
                return Task.FromResult<IActionResult>(BaseNotFoundView());
            }
            else
            {
                return Task.FromResult<IActionResult>(Redirect("~/Shared/Error"));
            }
        }

        [HttpGet]
        public Task<IActionResult> Search(string q)
        {
            return Task.FromResult<IActionResult>(View());
        }

        [HttpGet]
        public Task<IActionResult> SearchFAQ(string q)
        {
            return Task.FromResult<IActionResult>(View());
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
        public Task<IActionResult> SendContentUrlNotification(string name, string url, int type= (int)SendContentUrlType.Article)
        {
            ViewBag.Url = url;
            ViewBag.Name = name;

            return Task.FromResult<IActionResult>(PartialView("_SendContentUrlNotification", new SendContentUrlNotificationModel
            {
                Url = url,
                Name = name,
                Type = (SendContentUrlType) type,
            }));
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

        public async Task<FileResult> GoogleProductsFeed()
        {
            var data = await _productService.GetSkuGoogleItemsReportFile();

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = FileConstants.GOOGLE_PRODUCTS_FEED
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [HttpGet]
        public async Task<Result<bool>> SubscribeBronto(string id)
        {
            var toReturn = await _brontoService.Subscribe(id);
            return toReturn;
        }
    }
}