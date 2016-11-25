using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Users;

namespace VitalChoice.Core.Infrastructure.Helpers
{
    public static class AuthorizationTokenExtensions
    {
        public static async Task<ClaimsPrincipal> AuthorizeFromCookie(this HttpContext context)
        {
            try
            {
                var encryptedCookie = context.Request.Cookies[CheckoutConstants.CustomerAuthToken].FromHexString();
                if (encryptedCookie != null && encryptedCookie.Length > 0)
                {
                    context.Response.Cookies.Delete(CheckoutConstants.CustomerAuthToken);
                    var encryptionHost = context.RequestServices.GetRequiredService<IObjectEncryptionHost>();
                    var authData = encryptionHost.LocalDecrypt<ClientAuthorization>(encryptedCookie);
                    if (authData != null)
                    {
                        var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
                        var currentToken =
                            await tokenService.GetValidToken(authData.AuthToken, TokenType.CustomerAutoReLoginToken);
                        if (currentToken != null)
                        {
                            int idCustomer;
                            if (int.TryParse(currentToken.Data, out idCustomer))
                            {
                                var userService = context.RequestServices.GetRequiredService<IStorefrontUserService>();
                                var user = await userService.GetAsync(idCustomer);
                                if (user != null && user.IsConfirmed && user.Status == UserStatus.Active)
                                {
                                    var wholeHashCandidate =
                                        encryptionHost.HashBytes(
                                            Encoding.Unicode.GetBytes(currentToken.IdToken.ToString("N") + user.PasswordHash));
                                    if (wholeHashCandidate.AreEqualsTo(authData.WholeHash))
                                    {
                                        user = await userService.SignInAsync(user);
                                        await context.SpinAuthorizationToken(tokenService, currentToken, user, encryptionHost);
                                        var cartUid = context.GetCartUid();
                                        var checkoutService = context.RequestServices.GetRequiredService<ICheckoutService>();
                                        if (cartUid == null || await checkoutService.GetCartItemsCount(cartUid.Value) == 0)
                                        {
                                            var cart = await checkoutService.GetOrCreateCart(null, user.Id);
                                            context.SetCartUid(cart.CartUid);
                                        }
                                        var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                                        return await signInManager.CreateUserPrincipalAsync(user);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<CustomerAutoReloginFilter>();
                logger.LogWarning(e.ToString());
                return null;
            }
            return null;
        }

        public static async Task RemoveAuthorizationToken(this HttpContext context)
        {
            var encryptedCookie = context.Request.Cookies[CheckoutConstants.CustomerAuthToken].FromHexString();
            if (encryptedCookie != null && encryptedCookie.Length > 0)
            {
                context.Response.Cookies.Delete(CheckoutConstants.CustomerAuthToken);
                var encryptionHost = context.RequestServices.GetRequiredService<IObjectEncryptionHost>();
                ClientAuthorization authData;
                try
                {
                    authData = encryptionHost.LocalDecrypt<ClientAuthorization>(encryptedCookie);
                }
                catch
                {
                    authData = null;
                }
                if (authData != null)
                {
                    var tokenService = context.RequestServices.GetRequiredService<ITokenService>();
                    await tokenService.ExpireToken(authData.AuthToken);
                }
            }
        }

        public static async Task<Token> SpinAuthorizationToken(this HttpContext context, ITokenService tokenService, Token currentToken,
            ApplicationUser user, IObjectEncryptionHost encryptionHost)
        {
            if (currentToken == null)
            {
                try
                {
                    var encryptedCookie = context.Request.Cookies[CheckoutConstants.CustomerAuthToken].FromHexString();
                    if (encryptedCookie != null && encryptedCookie.Length > 0)
                    {
                        var authData = encryptionHost.LocalDecrypt<ClientAuthorization>(encryptedCookie);
                        if (authData != null)
                        {
                            currentToken =
                                await tokenService.GetValidToken(authData.AuthToken, TokenType.CustomerAutoReLoginToken);
                            if (currentToken != null)
                            {
                                await tokenService.ExpireToken(currentToken.IdToken);
                            }
                        }
                    }
                }
                catch
                {
                    currentToken = null;
                }
            }
            else
            {
                await tokenService.ExpireToken(currentToken.IdToken);
            }
            var resultToken = await tokenService.CreateTokenAsync(user.Id.ToString(),
                TimeSpan.FromDays(360), TokenType.CustomerAutoReLoginToken);
            var newAuthorization = new ClientAuthorization
            {
                AuthToken = resultToken.IdToken,
                WholeHash = encryptionHost.HashBytes(Encoding.Unicode.GetBytes(resultToken.IdToken.ToString("N") + user.PasswordHash))
            };
            context.Response.Cookies.Delete(CheckoutConstants.CustomerAuthToken);
            context.Response.Cookies.Append(CheckoutConstants.CustomerAuthToken,
                encryptionHost.LocalEncrypt(newAuthorization).ToHexString(), new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(360)
                });
            return resultToken;
        }
    }
}