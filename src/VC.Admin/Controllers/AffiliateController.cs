using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models;
using VC.Admin.Models.Product;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using System.Security.Claims;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Order;
using VC.Admin.Models.Order;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using VitalChoice.Domain.Transfer.Affiliates;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Affiliates)]
    public class AffiliateController : BaseApiController
    {
        private readonly IAffiliateService _affiliateService;
        private readonly IDynamicToModelMapper<AffiliateDynamic> _mapper;
        private readonly ILogger logger;

        public AffiliateController(IAffiliateService affiliateService,
            ILoggerProviderExtended loggerProvider, IDynamicToModelMapper<AffiliateDynamic> mapper)
        {
            this._affiliateService = affiliateService;
            _mapper = mapper;
            this.logger = loggerProvider.CreateLoggerDefault();
        }
        
        [HttpPost]
        public async Task<Result<PagedList<AffiliateListItemModel>>> GetAffiliates([FromBody]VAffiliateFilter filter)
        {
            var result = await _affiliateService.GetAffiliatesAsync(filter);

            var toReturn = new PagedList<AffiliateListItemModel>
            {
                Items = result.Items.Select(p => new AffiliateListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<AffiliateManageModel>> GetAffiliate(int id)
        {
            if (id == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new AffiliateManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    PaymentType=1,
                    Tier = 1,
                    CommissionFirst = 5,
                    CommissionAll = 5,
                };
            }

            var item = await _affiliateService.SelectAsync(id);
            AffiliateManageModel toReturn = _mapper.ToModel<AffiliateManageModel>(item);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<AffiliateManageModel>> UpdateAffiliate([FromBody]AffiliateManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = _mapper.FromModel(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            if (item.Id > 0)
            {
                item = await _affiliateService.UpdateAsync(item);
            }
            else
            {
                item = await _affiliateService.InsertAsync(item);
            }

            return _mapper.ToModel<AffiliateManageModel>(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteAffiliate(int id)
        {
            return await _affiliateService.DeleteAsync(id);
        }
    }
}