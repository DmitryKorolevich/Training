using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Infrastructure;
using VitalChoice.Domain;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Validation.Base;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
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
        public Result<RestrictedReferenceData> GetReferenceData()
        {
            var referenceData = this.appInfrastructureService.Get();

	        RestrictedReferenceData referenceDataModel;

			if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
	        {
		        referenceDataModel = new FullReferenceDataModel()
		        {
			        Roles = referenceData.Roles,
			        UserStatuses = referenceData.UserStatuses,
			        ContentTypes = referenceData.ContentTypes,
			        ContentProcessors = referenceData.ContentProcessors,
			        Labels = referenceData.Labels,
			        PublicHost = referenceData.PublicHost,
			        ContentItemStatusNames = referenceData.ContentItemStatusNames,
			        ProductCategoryStatusNames = referenceData.ProductCategoryStatusNames,
                    GCTypes = referenceData.GCTypes,
                    RecordStatuses = referenceData.RecordStatuses,
                };
	        }
			else
			{
				referenceDataModel = new RestrictedReferenceData()
				{
					Labels = referenceData.Labels,
				};
			}

	        return referenceDataModel;
        }
    }
}
