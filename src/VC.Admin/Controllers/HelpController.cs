using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models;
using VC.Admin.Models.Product;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using System.Security.Claims;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VC.Admin.Models.Order;
using VitalChoice.Interfaces.Services.Affiliates;
using VC.Admin.Models.Affiliate;
using VitalChoice.Domain.Transfer.Affiliates;
using System.Text;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Transfer.Help;
using VC.Admin.Models.Help;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Help)]
    public class HelpController : BaseApiController
    {
        private readonly IHelpService _helpService;
        private readonly ILogger logger;

        public HelpController(IHelpService helpService, ILoggerProviderExtended loggerProvider)
        {
            this._helpService = helpService;
            this.logger = loggerProvider.CreateLoggerDefault();
        }
        
        [HttpPost]
        public async Task<Result<PagedList<HelpTicketListItemModel>>> GetHelpTickets([FromBody]VHelpTicketFilter filter)
        {
            var result = await _helpService.GetHelpTicketsAsync(filter);

            var toReturn = new PagedList<HelpTicketListItemModel>
            {
                Items = result.Items.Select(p => new HelpTicketListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<HelpTicketManageModel>> GetHelpTicket(int id)
        {
            if (id == 0)
            {
                return new HelpTicketManageModel(null)
                {
                    StatusCode = RecordStatusCode.Active,
                    Priority=HelpTicketPriority.Medium,
                };
            }

            var result = await _helpService.GetHelpTicketAsync(id);

            return new HelpTicketManageModel(result);
        }

        [HttpPost]
        public async Task<Result<HelpTicketManageModel>> UpdateHelpTicket([FromBody]HelpTicketManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            
            item = await _helpService.UpdateHelpTicketAsync(item);

            return new HelpTicketManageModel(item);
        }        

        [HttpPost]
        public async Task<Result<bool>> DeleteHelpTicket(int id)
        {
            return await _helpService.DeleteHelpTicketAsync(id);
        }


        [HttpGet]
        public async Task<Result<HelpTicketCommentManageModel>> GetHelpTicketComment(int id)
        {
            if (id == 0)
            {
                return new HelpTicketCommentManageModel(null)
                {
                    StatusCode = RecordStatusCode.Active,
                };
            }

            var result = await _helpService.GetHelpTicketCommentAsync(id);

            return new HelpTicketCommentManageModel(result);
        }

        [HttpPost]
        public async Task<Result<HelpTicketCommentManageModel>> UpdateHelpTicketComment([FromBody]HelpTicketCommentManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();

            item = await _helpService.UpdateHelpTicketCommentAsync(item);

            return new HelpTicketCommentManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteHelpTicketComment(int id)
        {
            return await _helpService.DeleteHelpTicketCommentAsync(id);
        }
    }
}