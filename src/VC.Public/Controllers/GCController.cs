using System;
using System.Collections.Generic;
using System.Globalization;
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
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using VC.Public.Models.GC;

namespace VC.Public.Controllers
{
	[CustomerAuthorize]
	public class GCController : BaseMvcController
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IGcService _gcService;

		public GCController(IHttpContextAccessor contextAccessor, IGcService gcService)
		{
			_contextAccessor = contextAccessor;
            _gcService = gcService;
		}

        [HttpGet]
        public async Task<Result<GCInfoModel>> GetGCInfo(string id)
        {
            GCInfoModel toReturn = null;
            if (!String.IsNullOrEmpty(id))
            {
                GCFilter filter = new GCFilter();
                filter.Code = id;
                filter.Paging.PageItemCount = 1;
                var gc = (await _gcService.GetGiftCertificatesAsync(filter)).Items.FirstOrDefault();
                if(gc!=null)
                {
                    toReturn = new GCInfoModel(gc);
                }
            }
            return toReturn;
        }
    }
}