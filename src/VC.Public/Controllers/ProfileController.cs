using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VitalChoice.Core.Base;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;

namespace VC.Public.Controllers
{
	[Authorize]
    public class ProfileController : BaseMvcController
	{
		public ProfileController()
		{
		}

		public IActionResult Index()
        {
            return View();
        }
	}
}