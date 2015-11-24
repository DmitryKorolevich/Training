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

namespace VC.Public.Controllers
{
    [AllowAnonymous]
    public class HelpController : BaseMvcController
    {
        private const string RequestCatalogTempData = "request-catalog-messsage";

        private readonly IEcommerceDynamicService<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> _catalogRequestAddressService;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly IDynamicMapper<AddressDynamic, CatalogRequestAddress> _catalogRequestAddressMapper;
        private readonly ReCaptchaValidator _reCaptchaValidator;
        private readonly ILogger _logger;

        public HelpController(
            IEcommerceDynamicService<AddressDynamic, CatalogRequestAddress, AddressOptionType, CatalogRequestAddressOptionValue> catalogRequestAddressService,
            IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<AddressDynamic, CatalogRequestAddress> catalogRequestAddressMapper,
            ReCaptchaValidator reCaptchaValidator,
            ILoggerProviderExtended loggerProvider)
        {
            _catalogRequestAddressService = catalogRequestAddressService;
            _appInfrastructureService = appInfrastructureService;
            _catalogRequestAddressMapper = catalogRequestAddressMapper;
            _reCaptchaValidator = reCaptchaValidator;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpGet]
        public async Task<IActionResult> RequestCatalog()
        {
            if (TempData.ContainsKey(RequestCatalogTempData))
            {
                ViewBag.SuccessMessage = TempData[RequestCatalogTempData];
            }
            return View(GetCatalogRequestAddressPrototype());
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
    }
}