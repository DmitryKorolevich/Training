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
using System.IO;
using Microsoft.AspNet.Mvc.ViewEngines;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace VC.Public.Controllers
{
    [AllowAnonymous]
    public class VitalGreenController : BaseMvcController
    {
        private const string VITAL_GREEN_COOKIE_NAME = "VitalGreen";
        private const int VITAL_GREEN_COOKIE_EXPIRED_HOURS = 24;

        private readonly IStorefrontUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICountryService _countryService;
        private readonly IVitalGreenService _vitalGreenService;
        private readonly IFedExService _fedExService;
        private readonly ILogger _logger;

        public VitalGreenController(IStorefrontUserService userService,
                                    IHttpContextAccessor contextAccessor,
                                    ICountryService countryService,
                                    IVitalGreenService vitalGreenService,
                                    IFedExService fedExService,
                                    ILoggerProviderExtended loggerProvider)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
            _countryService = countryService;
            _vitalGreenService = vitalGreenService;
            _fedExService = fedExService;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpGet]
        public async Task<IActionResult> Step1()
        {
            await SetStates();
            var request = GetRequestFromCookie();
            return View(request ?? new VitalGreenRequestModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step1(VitalGreenRequestModel model)
        {
            if (!Validate(model))
            {
                await SetStates();
                return View(model);
            }

            var cookies = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model)));
            Response.Cookies.Append(VITAL_GREEN_COOKIE_NAME, cookies, new CookieOptions()
            {
                Expires = DateTime.Now.AddHours(VITAL_GREEN_COOKIE_EXPIRED_HOURS),
            });

            return RedirectToAction("ShipTo");
        }

        [HttpGet]
        public async Task<IActionResult> ShipTo()
        {
            var request = GetRequestFromCookie();

            if (request == null)
            {
                return RedirectToAction("Step1");
            }

            FedExZone zone = await _vitalGreenService.GetFedExZone(request.StateCode);
            if (zone == null)
            {
                ModelState.AddModelError("", "No Zone Found.");
            }
            if (zone.Id == 13)//13
            {
                ModelState.AddModelError("", "No Recyclers yet.");
                zone = null;
            }

            return View(zone);
        }

        [HttpGet]
        public async Task<IActionResult> Step2(int zoneid)
        {
            var requestModel = GetRequestFromCookie();
            if (requestModel == null)
            {
                return RedirectToAction("Step1");
            }

            FedExZone zone = await _vitalGreenService.GetFedExZone(zoneid);
            if (zone == null)
            {
                return RedirectToAction("Step1");
            }

            VitalGreenStep2Model model = null;
            try
            {
                var request = requestModel.Convert();
                request.ZoneId = zoneid;
                var url = _fedExService.CreateLabel(request, zone);
                if (!String.IsNullOrEmpty(url))
                {
                    var locations = _fedExService.GetDropoffLocations(request);
                    if (locations != null)
                    {
                        model = new VitalGreenStep2Model();
                        model.Url = url;
                        model.Locations = locations.ToList();

                        await _vitalGreenService.InsertRequest(request);
                        Response.Cookies.Append(VITAL_GREEN_COOKIE_NAME,String.Empty,new CookieOptions()
                        {
                            Expires=DateTime.Now.AddDays(-1)
                        });
                    }
                }
            }
            catch (AppValidationException e)
            {
                foreach (var message in e.Messages)
                {
                    ModelState.AddModelError("", message.Message);
                }
            }

            return View(model);
        }

        #region Private

        [NonAction]
        private async Task SetStates()
        {
            CountryFilter filter = new CountryFilter();
            filter.CountryCode = "US";
            ViewBag.States = (await _countryService.GetCountriesAsync(filter)).SingleOrDefault().States.Select(p => new SelectListItem()
            {
                Text = p.StateName,
                Value = p.StateCode,
            }).ToList();
        }

        [NonAction]
        private VitalGreenRequestModel GetRequestFromCookie()
        {
            var cookies = Request.Cookies[VITAL_GREEN_COOKIE_NAME];
            VitalGreenRequestModel model = null;
            if (!String.IsNullOrEmpty(cookies))
            {
                try
                {
                    cookies = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookies));
                    model = JsonConvert.DeserializeObject<VitalGreenRequestModel>(cookies);
                }
                catch (Exception e)
                {
                    Response.Cookies.Delete(VITAL_GREEN_COOKIE_NAME);
                    _logger.LogInformation(e.ToString());
                }
            }
            return model;
        }

        #endregion
    }
}