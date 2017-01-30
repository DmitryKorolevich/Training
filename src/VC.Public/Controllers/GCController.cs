using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Validation.Models;
using VC.Public.Models.GC;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Public.Controllers
{
	[CustomerAuthorize]
    [CustomerStatusCheck]
    public class GCController : BaseMvcController
	{
		private readonly IGcService _gcService;

		public GCController(IGcService gcService)
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
                filter.ExactCode = id;
                filter.Paging.PageItemCount = 1;
                var gc = (await _gcService.GetGiftCertificatesAsync(filter)).Items.FirstOrDefault();
                if(gc!=null)
                {
                    toReturn = new GCInfoModel(gc);
                }
            }
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<ICollection<GCInfoModel>>> GetGCsInfo([FromBody]ICollection<string> codes)
        {
            var gcs = await _gcService.TryGetGiftCertificatesAsync(codes);
            return gcs.Select(p=> new GCInfoModel(p)).ToList();
        }
    }
}