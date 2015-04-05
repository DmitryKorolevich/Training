﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Infrastructure;
using VitalChoice.Infrastructure;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Entities.Localization.Groups;
using Microsoft.AspNet.Authorization;
using VitalChoice.Admin.Models;

namespace VitalChoice.Admin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
	    private readonly ICommentService commentService;

	    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICommentService commentService)
        {
		    this.commentService = commentService;
		    UserManager = userManager;
            SignInManager = signInManager;

			
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }
        public SignInManager<ApplicationUser> SignInManager { get; private set; }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                var signInStatus = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
	            if (signInStatus.Succeeded)
	            {
		            return RedirectToLocal(returnUrl);
	            }
	            else
	            {
					ModelState.AddModelError("", "Invalid username or password.");
					return View(model);
				}
            }

			commentService.InsertWithUser(new Comment());

	      /*  var comments = new List<Comment>();

	        var comment = new Comment()
	        {
		        //Id = (new Random()).Next(1, 10000000),
		        CreationDate = DateTime.Now,
		        Text = "atatatatatat",
		        ObjectState = ObjectState.Added
	        };

			comments.Add(comment);

			var user = new ApplicationUser { UserName = "asdasd" + (new Random()).Next(1, 10000000).ToString(), CustomerId = 1, ObjectState = ObjectState.Added, Comments = comments };
			var signInStatus1 =  await UserManager.CreateAsync(user, model.Password);*/

			// If we got this far, something failed, redisplay form
			return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, CustomerId = 1};
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }



            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage
        [HttpGet]
        public IActionResult Manage(ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageUserViewModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();
				//user.ObjectState = ObjectState.Modified;
                var result = await UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            SignInManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByNameAsync(Context.User.Identity.Name);
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            Error
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



        #endregion
    }
}