using System;
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
using System.IO;
using VitalChoice.Ecommerce.Utils;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Public.Controllers
{
    [AllowAnonymous]
    public class VitalGreenController : BaseMvcController
    {
        public const string VITAL_GREEN_COOKIE_NAME = "VitalGreen";
        private const int VITAL_GREEN_COOKIE_EXPIRED_HOURS = 24;

        private readonly IVitalGreenService _vitalGreenService;
        private readonly IFedExService _fedExService;
        private readonly ILogger _logger;

        public VitalGreenController(IVitalGreenService vitalGreenService,
                                    IFedExService fedExService,
                                    ILoggerProviderExtended loggerProvider,
                                    IPageResultService pageResultService) : base(pageResultService)
        {
            _vitalGreenService = vitalGreenService;
            _fedExService = fedExService;
            _logger = loggerProvider.CreateLogger<VitalGreenController>();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Step1(VitalGreenRequestModel model)
        {
            string cookies;
            if (!Validate(model))
            {
                return Task.FromResult<IActionResult>(PartialView("_Step1", model));
            }

            cookies = Convert.ToBase64String(Encoding.UTF8.GetBytes(model.ToJson()));
            Response.Cookies.Append(VITAL_GREEN_COOKIE_NAME, cookies, new CookieOptions()
            {
                Expires = DateTime.Now.AddHours(VITAL_GREEN_COOKIE_EXPIRED_HOURS),
            });

            return Task.FromResult<IActionResult>(RedirectToAction("ShipTo"));
        }

        [HttpGet]
        public async Task<IActionResult> ShipTo()
        {
            var request = GetRequestFromCookie();

            if (request == null)
            {
                return Redirect("/content/vitalgreen");
            }

            FedExZone zone = await _vitalGreenService.GetFedExZone(request.StateCode);
            if (zone == null)
            {
                ModelState.AddModelError("", "No Zone Found.");
            }
            if (zone!=null && zone.Id == 13)//13
            {
                ModelState.AddModelError("", "No Recyclers yet.");
                zone = null;
            }

            return PartialView("_ShipTo",zone);
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

            return PartialView("_Step2", model);
        }

        #region Private

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
                    model = ((string) cookies).FromJson<VitalGreenRequestModel>();
                }
                catch (Exception e)
                {
                    Response.Cookies.Delete(VITAL_GREEN_COOKIE_NAME);
                    _logger.LogWarning(e.ToString());
                }
            }
            return model;
        }

        #endregion
    }
}