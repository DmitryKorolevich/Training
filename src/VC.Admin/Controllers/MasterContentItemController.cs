using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Controllers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Validation.Helpers.GlobalFilters;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.Users;
using VitalChoice.Validation.Logic;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Core;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Business.Services.Contracts;

namespace VitalChoice.Admin.Controllers
{
    public class MasterContentController : BaseApiController
    {
        private readonly ILogViewService logViewService;
        private readonly IContentService contentService;

        public MasterContentController(IContentService contentService, ILogViewService logViewService)
        {
            this.logViewService = logViewService;
            this.contentService = contentService;
        }

        [HttpGet]
        public Result<IEnumerable<ContentTypeEntity>> GetContentTypes()
        {
            IEnumerable<ContentTypeEntity> toReturn = null;
            return toReturn.ToList();
        }
    }
}