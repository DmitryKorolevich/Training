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
using Microsoft.Framework.Logging;
using System.Threading.Tasks;
using VitalChoice.Models.ContentManagement;

namespace VitalChoice.Admin.Controllers
{
    public class ContentController : BaseApiController
    {
        private readonly IGeneralContentService generalContentService;
        private readonly IMasterContentService masterContentService;
        private readonly ILogger logger;

        public ContentController(IGeneralContentService generalContentService, IMasterContentService masterContentService)
        {
            this.generalContentService = generalContentService;
            this.masterContentService = masterContentService;
            this.logger = LoggerService.GetDefault();
        }

        #region Common

        [HttpGet]
        public async Task<Result<IEnumerable<ContentTypeEntity>>> GetContentTypesAsync()
        {
            return (await generalContentService.GetContentTypesAsync()).ToList();
        }

        [HttpGet]
        public async Task<Result<IEnumerable<ContentProcessor>>> GetContentProcessorsAsync()
        {
            return (await generalContentService.GetContentProcessorsAsync()).ToList();
        }

        #endregion

        #region MasterContent

        [HttpPost]
        public async Task<Result<IEnumerable<MasterContentItemListItemModel>>> GetMasterContentItemsAsync([FromBody]MasterContentItemListFilter filter)
        {
            return (await masterContentService.GetMasterContentItemsAsync(filter.Type)).Select(p=>new MasterContentItemListItemModel(p)).ToList();
        }

        [HttpGet]
        public async Task<Result<ManageMasterContentItemModel>> GetMasterContentItemAsync(int id)
        {
            return new ManageMasterContentItemModel((await masterContentService.GetMasterContentItemAsync(id)));
        }

        [HttpPut]
        public async Task<Result<int?>> UpdateMasterContentItemAsync([FromBody]ManageMasterContentItemModel model, int? id = null)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item.Id = id.HasValue ? id.Value : 0;
            item = await masterContentService.UpdateMasterContentItemAsync(item);

            return item?.Id;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteMasterContentItemAsync(int id)
        {
            return await masterContentService.DeleteMasterContentItemAsync(id);
        }

        #endregion
    }
}