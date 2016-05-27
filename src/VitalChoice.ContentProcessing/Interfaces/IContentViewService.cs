using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.ContentProcessing.Interfaces
{
    public interface IContentViewService
    {
        Task<ContentViewModel> GetContentAsync(ControllerContext context, ClaimsPrincipal user,
            object additionalParameters = null);
    }
}