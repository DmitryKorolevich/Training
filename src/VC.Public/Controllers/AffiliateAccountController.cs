using System;
using System.Security.Claims;
using System.Threading.Tasks;
using VC.Public.Models.Auth;
using VitalChoice.Core.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Public.Models.Affiliate;
using VitalChoice.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Public.Controllers
{
    [AllowAnonymous]
    public class AffiliateAccountController : BaseMvcController
    {
        private readonly IAffiliateUserService _userService;
        private readonly IAffiliateService _affiliateService;
        private readonly ExtendedUserManager _userManager;
        private readonly ReferenceData _referenceData;
        private readonly IDynamicMapper<AffiliateDynamic, Affiliate> _affiliateMapper;

        public AffiliateAccountController(
            IAffiliateUserService userService,
            IDynamicMapper<AffiliateDynamic, Affiliate> affiliateMapper,
            IAffiliateService affiliateService, ExtendedUserManager userManager, ReferenceData referenceData)
        {
            _userService = userService;
            _affiliateMapper = affiliateMapper;
            _affiliateService = affiliateService;
            _userManager = userManager;
            _referenceData = referenceData;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string alreadyTakenEmail = null, bool forgot=false)
        {
            if(await AffiliateAuthorizeAttribute.IsAuthenticated(Request.HttpContext))
            {
                return RedirectToAction("Index", "AffiliateProfile");
            }

            if (!string.IsNullOrWhiteSpace(alreadyTakenEmail))
            {
                ViewBag.AlreadyTakenEmail = alreadyTakenEmail;
            }
            ViewBag.ForgotPassSuccess = forgot;

            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                ApplicationUser user = null;
                var id = await _affiliateService.GetAffiliateId(model.Email);
                if (id.HasValue)
                {
                    user = await _userService.SignInAsync(id.Value, model.Password);
                }
                if (user == null)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                }
            }
            catch(AffiliatePendingException)
            {
                return View("Pending");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "AffiliateProfile");
        }

        public async Task<IActionResult> Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                int id;
                ApplicationUser user = null;
                if (int.TryParse(_userManager.GetUserId(User), out id))
                {
                    user = await _userService.FindAsync(id);
                }
                if (user == null)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser])
                    {
                        ViewName = "Login"
                    };
                }

                await _userService.SignOutAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Activate(Guid id)
        {
            ApplicationUser result;
            try
            {
                result = await _userService.GetByTokenAsync(id);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken])
                {
                    ViewName = "Login"
                };
            }
			if (result.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed])
                {
                    ViewName = "Login"
                };
			}

			return View(new CreateAccountModel()
            {
                Email = result.Email,
                PublicId = result.PublicId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(CreateAccountModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetAsync(model.PublicId);
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            var affiliate = await _affiliateService.SelectAsync(user.Id);
            if (affiliate == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            if (affiliate.StatusCode == (int)CustomerStatus.Suspended || affiliate.StatusCode == (int)CustomerStatus.Deleted)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
            }

            affiliate.Email = model.Email;
            affiliate.StatusCode = (int)CustomerStatus.Active;

            await _affiliateService.UpdateAsync(affiliate, model.Password);

            return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            ApplicationUser result;
            try
            {
                result = await _userService.GetByTokenAsync(id);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken])
                {
                    ViewName = "Login"
                };
            }

            return View(new ResetPasswordModel()
            {
                Token = id.ToString(),
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser user = null;
            var id = await _affiliateService.GetAffiliateId(model.Email);
            if (id.HasValue)
            {
                user = await _userService.ResetPasswordAsync(id.Value, model.Token, model.Password);
            }
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            await _userService.SignInAsync(user);

            return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordEmailModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordEmailModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            ApplicationUser user = null;
            var id = await _affiliateService.GetAffiliateId(model.Email);
            if (id.HasValue)
            {
                user = await _userService.FindAsync(id.Value);
            }
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            await _userService.SendForgotPasswordAsync(user.PublicId);

            return RedirectToAction("Login",new { forgot = true });
        }

        [HttpGet]
        public IActionResult Register()
        {
            AffiliateManageModel model = new AffiliateManageModel();
            InitRegisterModel(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AffiliateManageModel model)
        {
            InitRegisterModel(model, true);
            if (!model.IsAllowAgreement)
            {
                ModelState.AddModelError(String.Empty, "Please agree to Web Affiliate Agreement.");
            }
            if (!model.IsNotSpam)
            {
                ModelState.AddModelError(String.Empty, "Please agree to SPAM Agreement.");
            }
            if (!Validate(model))
            {
                return View(model);
            }

            try
            {
                var validated = await _userService.ValidateEmailUniquenessAsync(model.Email);
                if (!validated)
                {
                    return RedirectToAction("Login", new { alreadyTakenEmail = model.Email });
                }
                
                var affiliate = await _affiliateMapper.FromModelAsync(model);

                affiliate.StatusCode = (int)AffiliateStatus.Pending;
                affiliate.CommissionAll = AffiliateConstants.DefaultCommissionAll;
                affiliate.CommissionFirst = AffiliateConstants.DefaultCommissionFirst;
                affiliate.Data.Tier = AffiliateConstants.DefaultTier;

                affiliate = await _affiliateService.InsertAsync(affiliate, model.Password);
                if (affiliate == null || affiliate.Id == 0)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
                }

                await _userService.SendSuccessfulRegistration(model.Email, model.Name, String.Empty);
            }
            catch (AppValidationException e)
            {
                foreach (var message in e.Messages)
                {
                    ModelState.AddModelError(message.Field, message.Message);
                }
                return View(model);
            }

            return View("Pending");
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsAffiliate(Guid id)
        {
            ApplicationUser result;
            try
            {
                result = await _userService.GetByTokenAsync(id);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin])
                {
                    ViewName = "Login"
                };
            }

            result.ConfirmationToken = Guid.Empty;
            try
            {
                await _userService.UpdateAsync(result);

                result = await _userService.SignInAsync(result);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn])
                {
                    ViewName = "Login"
                };
            }

            return RedirectToAction("Index", "AffiliateProfile");
        }

        private void InitRegisterModel(AffiliateManageModel model,bool refresh=false)
        {
            var settings = _referenceData;
            model.MonthlyEmailsSentOptions = settings.AffiliateMonthlyEmailsSentOptions.Select(p => new SelectListItem()
            {
                Value = p.Key.ToString(),
                Text = p.Text,
            }).ToList();
            model.ProfessionalPracticeOptions = settings.AffiliateProfessionalPractices.Select(p => new SelectListItem()
            {
                Value = p.Key.ToString(),
                Text = p.Text,
            }).ToList();
            model.CommissionAll = AffiliateConstants.DefaultCommissionAll;
            model.CommissionFirst = AffiliateConstants.DefaultCommissionFirst;
            if (!refresh)
            {
                model.PaymentType = AffiliateConstants.DefaultPaymentType;//Credit
                if (settings.DefaultCountry != null)
                {
                    model.IdCountry = settings.DefaultCountry.Id;
                }
            }
        }
    }
}