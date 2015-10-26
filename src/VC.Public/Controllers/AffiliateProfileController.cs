using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Newtonsoft.Json;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Interfaces.Services.Affiliates;

namespace VC.Public.Controllers
{
	[AffiliateAuthorize]
	public class AffiliateProfileController : BaseMvcController
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IAffiliateUserService _affiliateUserService;
		private readonly IAffiliateService _affiliateService;
		private readonly IDynamicToModelMapper<CustomerAddressDynamic> _addressConverter;
		private readonly IDynamicToModelMapper<CustomerPaymentMethodDynamic> _paymentMethodConverter;

		public AffiliateProfileController(
            IHttpContextAccessor contextAccessor,
            IAffiliateUserService affiliateUserService,
			IAffiliateService affiliateService)
		{
			_contextAccessor = contextAccessor;
            _affiliateUserService = affiliateUserService;
            _affiliateService = affiliateService;
		}
        
        private int GetInternalAffiliateId()
		{
			var context = _contextAccessor.HttpContext;
			var internalId = Convert.ToInt32(context.User.GetUserId());

			return internalId;
		}

        [HttpGet]
        public IActionResult Index()
		{
			return View();
		}
	}
}