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
using VC.Public.Models.Affiliate;
using VitalChoice.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Mvc.Rendering;
using VitalChoice.Domain.Transfer.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Transfer.Base;
using cloudscribe.Web.Pagination;

namespace VC.Public.Controllers
{
	[AffiliateAuthorize]
	public class AffiliateProfileController : BaseMvcController
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IAffiliateUserService _affiliateUserService;
		private readonly IAffiliateService _affiliateService;
        private readonly IDynamicMapper<AffiliateDynamic, Affiliate> _affiliateMapper;

		public AffiliateProfileController(
            IHttpContextAccessor contextAccessor,
            IAffiliateUserService affiliateUserService,
			IAffiliateService affiliateService,
            IDynamicMapper<AffiliateDynamic, Affiliate> affiliateMapper)
		{
			_contextAccessor = contextAccessor;
            _affiliateUserService = affiliateUserService;
            _affiliateService = affiliateService;
            _affiliateMapper = affiliateMapper;
        }
        
        private int GetInternalAffiliateId()
		{
			var context = _contextAccessor.HttpContext;
			var internalId = Convert.ToInt32(context.User.GetUserId());

			return internalId;
		}

        private async Task<AffiliateDynamic> GetCurrentAffiliateDynamic()
        {
            var internalId = GetInternalAffiliateId();
            var affiliate = await _affiliateService.SelectAsync(internalId);
            if (affiliate == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            if (affiliate.StatusCode == (int)CustomerStatus.Suspended || affiliate.StatusCode == (int)CustomerStatus.Deleted)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
            }

            affiliate.IdEditedBy = null;

            return affiliate;
        }

        #region Affiliates

        [HttpGet]
        public IActionResult Index()
		{
			return View();
		}

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var context = _contextAccessor.HttpContext;

            var user = await _affiliateUserService.FindAsync(context.User.GetUserName());
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            await _affiliateUserService.UpdateWithPasswordChangeAsync(user, model.OldPassword, model.Password);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated];

            return View(new ChangePasswordModel());
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProfile()
        {
            var affiliate = await GetCurrentAffiliateDynamic();

            var model = _affiliateMapper.ToModel<AffiliateManageModel>(affiliate);
            model.CurrentEmail = model.Email;
            model.Email = String.Empty;

            return View(model);
        }

        private void CleanProfileEmailFields(AffiliateManageModel model)
        {
            model.ConfirmEmail = model.Email = string.Empty;
            ModelState["Email"] = new ModelStateEntry();
            ModelState["ConfirmEmail"] = new ModelStateEntry();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile(AffiliateManageModel model)
        {
            if (!Validate(model))
            {
                CleanProfileEmailFields(model);
                return View(model);
            }

            var affiliate = await GetCurrentAffiliateDynamic();

            try
            {
                var oldEmail = affiliate.Email;

                affiliate = model.Update(affiliate);

                affiliate = await _affiliateService.UpdateAsync(affiliate);

                if (oldEmail != affiliate.Email)
                {
                    var user = await _affiliateUserService.GetAsync(GetInternalAffiliateId());
                    if (user == null)
                    {
                        throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
                    }
                    await _affiliateUserService.RefreshSignInAsync(user);
                }

                ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated];

                model = _affiliateMapper.ToModel<AffiliateManageModel>(affiliate);
                model.CurrentEmail = model.Email;
                CleanProfileEmailFields(model);
            }
            catch (AppValidationException e)
            {
                foreach (var message in e.Messages)
                {
                    ModelState.AddModelError(message.Field, message.Message);
                }
            }

            return View(model);
        }

        #endregion

        #region AffiliatePayments

        [HttpGet]
        public async Task<IActionResult> PaymentHistory()
        {
            var toReturn = (await _affiliateService.GetAffiliatePayments(GetInternalAffiliateId())).Select(p => new PaymentHistoryLineItemModel()
            {
                DateCreated = p.DateCreated,
                Amount = p.Amount,
            }).ToList();
            return View(toReturn);
        }

        [HttpGet]
        public async Task<IActionResult> UnpaidOrders()
        {
            AffiliateOrderPaymentFilter filter = new AffiliateOrderPaymentFilter();
            filter.IdAffiliate = GetInternalAffiliateId();
            filter.Status = AffiliateOrderPaymentStatus.NotPaid;
            var result = await _affiliateService.GetAffiliateOrderPayments(filter);
            var toReturn = result.Items.Select(p => new OrderPaymentListItemModel(p)).ToList();
            return View(toReturn);
        }

        [HttpGet]
        public async Task<IActionResult> PaidOrders(DateTime? from=null, DateTime? to=null, int page=1)
        {
            AffiliateOrderPaymentFilter filter = new AffiliateOrderPaymentFilter();
            filter.IdAffiliate = GetInternalAffiliateId();
            filter.Status = AffiliateOrderPaymentStatus.Paid;
            filter.From = from;
            filter.To = to;
            filter.Paging = new Paging();
            filter.Paging.PageIndex = page;
            if(filter.Paging.PageIndex<1)
            {
                filter.Paging.PageIndex = 0;
            }
            filter.Paging.PageItemCount = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT;
            var result = await _affiliateService.GetAffiliateOrderPayments(filter);
            OrderPaymentsModel toReturn = new OrderPaymentsModel();
            toReturn.From = from;
            toReturn.To = to;
            toReturn.Count = result.Count;
            toReturn.Paging = new PaginationSettings()
            {
                CurrentPage = page,
                ItemsPerPage= BaseAppConstants.DEFAULT_LIST_TAKE_COUNT,
                TotalItems = result.Count,
            };
            toReturn.Items = result.Items.Select(p => new OrderPaymentListItemModel()
            {
                DateCreated = p.Order.DateCreated,
                IdOrder = p.Id,
                NewCustomerOrder = p.NewCustomerOrder,
                ProductTotal = p.Order.ProductsSubtotal,
                OrderTotal = p.Order.Total,
                Shipping = p.Order.ShippingTotal,
                Tax = p.Order.TaxTotal,
                Commission = p.Amount,
            }).ToList();
            return View(toReturn);
        }

        #endregion
    }
}