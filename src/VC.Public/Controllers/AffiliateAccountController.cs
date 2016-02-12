using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Auth;
using VitalChoice.Core.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Public.Models.Affiliate;
using VitalChoice.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Mvc.Rendering;
using VitalChoice.Core.Infrastructure;
using Microsoft.AspNet.Authorization;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using System.Linq;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.Controllers
{
    [AllowAnonymous]
    public class AffiliateAccountController : BaseMvcController
    {
        private readonly IAffiliateUserService _userService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAffiliateService _affiliateService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IDynamicMapper<AffiliateDynamic, Affiliate> _affiliateMapper;

        public AffiliateAccountController(
            IAffiliateUserService userService,
            IHttpContextAccessor contextAccessor,
            IDynamicMapper<AffiliateDynamic, Affiliate> affiliateMapper,
            IAffiliateService affiliateService)
        {
            _userService = userService;
            _contextAccessor = contextAccessor;
            _affiliateMapper = affiliateMapper;
            _affiliateService = affiliateService;
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
                var user = await _userService.SignInAsync(model.Email, model.Password);
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
            var context = _contextAccessor.HttpContext;

            if (context.User.Identity.IsAuthenticated)
            {
                var user = await _userService.FindAsync(context.User.GetUserName());
                if (user == null)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
                }

                await _userService.SignOutAsync(user);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Activate(Guid id)
        {
            var result = await _userService.GetByTokenAsync(id);
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
            }
			if (result.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
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

            await _userService.SendSuccessfulRegistration(model.Email, user.FirstName, user.LastName);

            return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            var result = await _userService.GetByTokenAsync(id);
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
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

            await _userService.ResetPasswordAsync(model.Email, model.Token, model.Password);

            var user = await _userService.FindAsync(model.Email);
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

            var user = await _userService.FindAsync(model.Email);
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

                //BUG: affiliate type 1?! WTF?
                var affiliate = await _affiliateMapper.FromModelAsync(model, 1);

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
            var result = await _userService.GetByTokenAsync(id);
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
            }

            result.ConfirmationToken = Guid.Empty;
            await _userService.UpdateAsync(result);

            result = await _userService.SignInAsync(result);
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
            }

            return RedirectToAction("ChangeProfile", "AffiliateProfile");
        }

        private void InitRegisterModel(AffiliateManageModel model,bool refresh=false)
        {
            var settings = HttpContext.RequestServices.GetService<IAppInfrastructureService>().Get();
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