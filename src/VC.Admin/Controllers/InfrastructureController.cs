using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Models.Infrastructure;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Models;

namespace VitalChoice.Controllers
{
    public class InfrastructureController : BaseApiController
    {
        private readonly IAppInfrastructureService appInfrastructureService;
        private readonly IHttpContextAccessor contextAccessor;

        public InfrastructureController(IAppInfrastructureService appInfrastructureService, IHttpContextAccessor contextAccessor)
        {
            this.appInfrastructureService = appInfrastructureService;
            this.contextAccessor = contextAccessor;
        }

        [HttpGet]
        public Result<FullReferenceDataModel> GetReferenceData()
        {
            var referenceData = this.appInfrastructureService.Get();

            // if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            FullReferenceDataModel referenceDataModel = new FullReferenceDataModel()
            {
                Roles = referenceData.Roles,
                UserStatuses = referenceData.UserStatuses,
                ContentTypes = referenceData.ContentTypes,
                ContentProcessors = referenceData.ContentProcessors,
                Labels = referenceData.Labels,
            };

            return referenceDataModel;
        }
    }
}
