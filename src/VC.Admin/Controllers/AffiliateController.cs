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
using VitalChoice.Interfaces.Services.Orders;
using VC.Admin.Models.Order;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using VitalChoice.Domain.Transfer.Affiliates;
using System.Text;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Constants;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Domain.Entities.Settings;
using VitalChoice.Domain.Transfer.Settings;
using Newtonsoft.Json;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Affiliates)]
    public class AffiliateController : BaseApiController
    {
        private readonly IAffiliateService _affiliateService;
        private readonly IDynamicMapper<AffiliateDynamic> _mapper;
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly Country _defaultCountry;
        private readonly ILogger logger;

        public AffiliateController(
            IAffiliateService affiliateService,
            IAffiliateUserService affiliateUserService,
            ILoggerProviderExtended loggerProvider,
            IDynamicMapper<AffiliateDynamic> mapper,
            IAppInfrastructureService appInfrastructureService,
            IObjectHistoryLogService objectHistoryLogService)
        {
            _affiliateService = affiliateService;
            _affiliateUserService = affiliateUserService;
            _mapper = mapper;
            _objectHistoryLogService = objectHistoryLogService;
            _defaultCountry = appInfrastructureService.Get().DefaultCountry;
            logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpPost]
        public async Task<Result<PagedList<VCustomerInAffiliate>>> GetCustomerInAffiliateReport([FromBody]FilterBase filter)
        {
            var result = await _affiliateService.GetCustomerInAffiliateReport(filter);

            var toReturn = new PagedList<VCustomerInAffiliate>
            {
                Items = result.Items,
                Count = result.Count,
            };

            return toReturn;
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
                    StatusCode = RecordStatusCode.NotActive,
                    IdCountry = _defaultCountry != null ? _defaultCountry.Id : (int?)null,
                    PaymentType = AffiliateConstants.DefaultPaymentType,//Credit
                    Tier = AffiliateConstants.DefaultTier,
                    CommissionFirst = AffiliateConstants.DefaultCommissionFirst,
                    CommissionAll = AffiliateConstants.DefaultCommissionAll,
                };
            }

            var item = await _affiliateService.SelectAsync(id);
            if (item == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }
            AffiliateManageModel toReturn = _mapper.ToModel<AffiliateManageModel>(item);

            var login = await _affiliateUserService.GetAsync(toReturn.Id);
            if (login == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
            }

            toReturn.IsConfirmed = login.IsConfirmed;
            toReturn.PublicUserId = login.PublicId;

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

            var toReturn = _mapper.ToModel<AffiliateManageModel>(item);

            toReturn.IsConfirmed = model.IsConfirmed;
            toReturn.PublicUserId = model.PublicUserId;

            return toReturn;
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
        public async Task<Result<bool>> DeleteAffiliate(int id, [FromBody] object model)
        {
            return await _affiliateService.DeleteAsync(id);
        }

        [HttpPost]
        public async Task<Result<bool>> ResendActivation(Guid id, [FromBody] object model)
        {
            await _affiliateUserService.ResendActivationAsync(id);

            return true;
        }

        [HttpPost]
        public async Task<Result<bool>> ResetPassword(Guid id, [FromBody] object model)
        {
            await _affiliateUserService.SendResetPasswordAsync(id);

            return true;
        }

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (AffiliateDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(AffiliateDynamic));
                var model = _mapper.ToModel<AffiliateManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (AffiliateDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(AffiliateDynamic));
                var model = _mapper.ToModel<AffiliateManageModel>(dynamic);
                toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }

            return toReturn;
        }
    }
}