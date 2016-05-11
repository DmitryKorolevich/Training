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
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using VC.Public.Models.GC;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Public.Controllers
{
	[CustomerAuthorize]
    [CustomerStatusCheck]
    public class GCController : BaseMvcController
	{
		private readonly IGcService _gcService;

		public GCController(IGcService gcService,
            IPageResultService pageResultService) : base(pageResultService)
		{
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