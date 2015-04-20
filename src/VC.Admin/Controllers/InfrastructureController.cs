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
        public Result<RestrictedReferenceData> GetReferenceData()
	    {
		    var referenceData = this.appInfrastructureService.Get();

			var referenceDataModel = new RestrictedReferenceData();

		    referenceDataModel.ValidationMessages =
			    LocalizationService.LocalizationData[1].Select(x => x.Value.FirstOrDefault(y => y.CultureId == "en"))
				    .Select(x=> new LookupItemModel<int>()
				    {
					    Key = x.ItemId,
						Text = x.Value
				    }).ToList(); //ItemID should be string - need to discuss
		   // if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
		    {
			    referenceDataModel = new FullReferenceDataModel()
			    {
					ValidationMessages = referenceDataModel.ValidationMessages,
				    Roles = referenceData.Roles.Select(x=> new LookupItemModel<string>()
				    {
					    Key = x.Key,
						Text = x.Text
				    }).ToList()
			    };
		    }

			return referenceDataModel;
	    }
    }
}
