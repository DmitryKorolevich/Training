﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Public.Models.Affiliate;
using cloudscribe.Web.Pagination;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Public.Controllers
{
    [AffiliateAuthorize]
    public class AffiliateProfileController : BaseMvcController
    {
        private readonly IAffiliateUserService _affiliateUserService;
        private readonly IAffiliateService _affiliateService;
        private readonly IDynamicMapper<AffiliateDynamic, Affiliate> _affiliateMapper;
        private readonly IOptions<AppOptions> _appOptions;
        private readonly ExtendedUserManager _userManager;

        public AffiliateProfileController(
            IAffiliateUserService affiliateUserService,
            IAffiliateService affiliateService,
            IDynamicMapper<AffiliateDynamic, Affiliate> affiliateMapper,
            IOptions<AppOptions> appOptions, ExtendedUserManager userManager)
        {
            _affiliateUserService = affiliateUserService;
            _affiliateService = affiliateService;
            _affiliateMapper = affiliateMapper;
            _appOptions = appOptions;
            _userManager = userManager;
        }

        private int GetInternalAffiliateId()
        {
            var internalId = Convert.ToInt32(_userManager.GetUserId(User));
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
        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> model = new Dictionary<string, string>();
            var internalId = GetInternalAffiliateId();
            var affiliate = await _affiliateService.SelectAsync(internalId);
            if (affiliate != null)
            {
                if (affiliate.SafeData.PromoteByDrSearsLEANCoachAmbassador == true)
                {
                    model.Add("LEAN", true.ToString());
                }
                model.Add("Id", affiliate.Id.ToString());
                model.Add("Name", affiliate.Name);
                model.Add("CommissionFirst", affiliate.CommissionFirst.ToString());
                model.Add("CommissionAll", affiliate.CommissionAll.ToString());
                model.Add("PublicHost", _appOptions.Value.PublicHost);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _affiliateUserService.FindAsync(_userManager.GetUserName(User));
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

            var model = await _affiliateMapper.ToModelAsync<AffiliateManageModel>(affiliate);
            model.CurrentEmail = model.Email;
            model.Email = String.Empty;

            return View(model);
        }

        private void CleanProfileEmailFields(AffiliateManageModel model)
        {
            model.ConfirmEmail = model.Email = string.Empty;
            ModelState.Remove("Email");
            ModelState.Remove("ConfirmEmail");
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile(AffiliateManageModel model)
        {
            if (!Validate(model))
            {
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

                model = await _affiliateMapper.ToModelAsync<AffiliateManageModel>(affiliate);
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
        public async Task<IActionResult> PaidOrders(DateTime? from = null, DateTime? to = null, int page = 1)
        {
            AffiliateOrderPaymentFilter filter = new AffiliateOrderPaymentFilter();
            filter.IdAffiliate = GetInternalAffiliateId();
            filter.Status = AffiliateOrderPaymentStatus.Paid;
            filter.From = from;
            filter.To = to;
            filter.Paging = new Paging();
            filter.Paging.PageIndex = page;
            if (filter.Paging.PageIndex < 1)
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
                ItemsPerPage = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT,
                TotalItems = result.Count,
            };
            toReturn.Items = result.Items.Select(p => new OrderPaymentListItemModel(p)).ToList();
            return View(toReturn);
        }

        [HttpGet]
        public IActionResult AdBanners()
        {
            ViewBag.IdAffiliate = GetInternalAffiliateId();
            return View();
        }

        #endregion
    }
}