using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Http.Extensions;

namespace VitalChoice.Core.GlobalFilters
{
    public class CustomValidateAntiforgeryTokenAuthorizationFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
    {
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger _logger;
        private static Action<ILogger,string,Exception> _antiforgeryTokenInvalidMessage = 
            LoggerMessage.Define<string>(
                LogLevel.Information,
                1,
                "Antiforgery token validation failed. {Message}");

        public CustomValidateAntiforgeryTokenAuthorizationFilter(IAntiforgery antiforgery, ILoggerFactory loggerFactory)
        {
            if (antiforgery == null)
            {
                throw new ArgumentNullException(nameof(antiforgery));
            }

            _antiforgery = antiforgery;
            _logger = loggerFactory.CreateLogger<CustomValidateAntiforgeryTokenAuthorizationFilter>();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (IsClosestAntiforgeryPolicy(context.Filters) && ShouldValidate(context))
            {
                try
                {
                    await _antiforgery.ValidateRequestAsync(context.HttpContext);
                }
                catch (AntiforgeryValidationException exception)
                {
                    _antiforgeryTokenInvalidMessage(_logger, exception.Message, exception);
                    context.Result = new RedirectResult(context.HttpContext.Request.GetEncodedUrl(), false);
                }
            }
        }

        protected virtual bool ShouldValidate(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return true;
        }

        private bool IsClosestAntiforgeryPolicy(IList<IFilterMetadata> filters)
        {
            // Determine if this instance is the 'effective' antiforgery policy.
            for (var i = filters.Count - 1; i >= 0; i--)
            {
                var filter = filters[i];
                if (filter is IAntiforgeryPolicy)
                {
                    return object.ReferenceEquals(this, filter);
                }
            }

            Debug.Fail("The current instance should be in the list of filters.");
            return false;
        }
    }
}