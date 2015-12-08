using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentViewService
    {
        Task<ContentViewModel> GetContentAsync(ActionContext context, ActionBindingContext bindingContext, ClaimsPrincipal user,
            object additionalParameters = null);
    }
}