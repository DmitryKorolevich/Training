﻿using System.Collections.Generic;
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
using VitalChoice.Interfaces.Services.Orders;
using VC.Admin.Models.Order;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using VitalChoice.Domain.Transfer.Affiliates;
using System.Text;

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
                    PaymentType=2,//Credit
                    Tier = 1,
                    CommissionFirst = 8,
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

        [HttpGet]
        public Task<Result<AffiliateEmailModel>> GetAffiliateEmail(int id)
        {
            AffiliateEmailModel toReturn = new AffiliateEmailModel();
            toReturn.FromName = "Vital Choice";
            toReturn.FromEmail = "affiliatesupport@vitalchoice.com";
            toReturn.Subject = "Your Vital Choice affiliate account is ready.";
            if (id == 1)//notify
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Dear {0},");
                builder.AppendLine();
                builder.AppendLine("Your Vital Choice Affiliate account is ready for use. Your User Name is your e-mail address ({1}).  You can log into your account at https://www.vitalchoice.com/shop/pc/AffiliateLogin.asp to retrieve special links of Vital Choice text and banner ads that contain your affiliate ID.");
                builder.AppendLine();
                builder.AppendLine("Program overview: http://www.vitalchoice.com/shop/pc/viewContent.asp?idpage=22");
                builder.AppendLine();
                builder.AppendLine("We appreciate your advocacy and interest in promoting wild seafood nutrition. Looking forward to working together.");
                builder.AppendLine();
                builder.AppendLine("Questions or comments? Contact us at affiliatesupport@vitalchoice.com");
                builder.AppendLine();
                builder.AppendLine("Kind regards,");
                builder.AppendLine();
                builder.AppendLine("The Vital Choice Team");
                toReturn.Message = builder.ToString();
            }
            else if (id == 2)//email
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Dear {0},");
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine();
                builder.AppendLine("Questions or comments? Contact us at affiliatesupport@vitalchoice.com");
                builder.AppendLine();
                builder.AppendLine("Kind regards,");
                builder.AppendLine();
                builder.AppendLine("The Vital Choice Team");
                toReturn.Message = builder.ToString();
            }

            return Task.FromResult<Result<AffiliateEmailModel>>(toReturn);
        }

        [HttpPost]
        public async Task<Result<bool>> SendAffiliateEmail([FromBody]AffiliateEmailModel model)
        {
            if (!Validate(model))
                return false;
            var item = model.Convert();
            return await _affiliateService.SendAffiliateEmailAsync(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteAffiliate(int id)
        {
            return await _affiliateService.DeleteAsync(id);
        }
    }
}