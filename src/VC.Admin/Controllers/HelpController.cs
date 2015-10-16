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
using Microsoft.Net.Http.Headers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Domain.Entities.Help;
using Microsoft.Framework.Primitives;
#if DNX451
using System.Net.Mime;
#endif
using VitalChoice.Domain.Entities.Roles;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Help)]
    public class HelpController : BaseApiController
    {
        private readonly IHelpService _helpService;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly ILogger _logger;

        public HelpController(IHelpService helpService, IAppInfrastructureService appInfrastructureService, ILoggerProviderExtended loggerProvider)
        {
            _helpService = helpService;
            _appInfrastructureService = appInfrastructureService;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        #region HelpsArea

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
                    Priority = TicketPriority.Medium,
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

            item = await _helpService.UpdateHelpTicketAsync(item, Int32.Parse(Request.HttpContext.User.GetUserId()));

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
            if (item != null)
            {
                item.IdEditedBy = Int32.Parse(Request.HttpContext.User.GetUserId());
            }

            item = await _helpService.UpdateHelpTicketCommentAsync(item);

            return new HelpTicketCommentManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteHelpTicketComment(int id)
        {
            return await _helpService.DeleteHelpTicketCommentAsync(id, Int32.Parse(Request.HttpContext.User.GetUserId()));
        }

        #endregion

        #region BugsArea

        [HttpPost]
        public async Task<Result<PagedList<BugTicketListItemModel>>> GetBugTickets([FromBody]VHelpTicketFilter filter)
        {
            var result = await _helpService.GetBugTicketsAsync(filter);

            var toReturn = new PagedList<BugTicketListItemModel>
            {
                Items = result.Items.Select(p => new BugTicketListItemModel(p)).ToList(),
                Count = result.Count,
            };

            var superAdmin = _appInfrastructureService.Get().AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdmin.Normalize());
            int userId = Int32.Parse(Request.HttpContext.User.GetUserId());
            foreach (var item in toReturn.Items)
            {
                if(isSuperAdmin || item.IdAddedBy == userId)
                {
                    item.AllowDelete = true;
                }
            }

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<BugTicketManageModel>> GetBugTicket(int id)
        {
            if (id == 0)
            {
                return new BugTicketManageModel(null)
                {
                    PublicId= Guid.NewGuid(),
                    StatusCode = RecordStatusCode.Active,
                    Priority = TicketPriority.Medium,
                    IsAllowEdit=true,
                    Comments = new List<BugTicketCommentManageModel>(),
                    Files = new List<FileModel>(),
                };
            }

            var result = await _helpService.GetBugTicketAsync(id);
            var toReturn= new BugTicketManageModel(result);

            var superAdmin = _appInfrastructureService.Get().AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            if(HttpContext.User.IsInRole(superAdmin.Normalize()) || result.IdAddedBy== Int32.Parse(Request.HttpContext.User.GetUserId()))
            {
                toReturn.IsAllowEdit = true;
            }

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<BugTicketManageModel>> UpdateBugTicket([FromBody]BugTicketManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();

            var superAdmin = _appInfrastructureService.Get().AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdmin.Normalize());
            item = await _helpService.UpdateBugTicketAsync(item, Int32.Parse(Request.HttpContext.User.GetUserId()), isSuperAdmin);

            return new BugTicketManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBugTicket(int id)
        {
            var superAdmin = _appInfrastructureService.Get().AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdmin.Normalize());

            return await _helpService.DeleteBugTicketAsync(id, isSuperAdmin ? (int?)null : Int32.Parse(Request.HttpContext.User.GetUserId()));
        }


        [HttpGet]
        public async Task<Result<BugTicketCommentManageModel>> GetBugTicketComment(int id)
        {
            if (id == 0)
            {
                return new BugTicketCommentManageModel(null)
                {
                    PublicId = Guid.NewGuid(),
                    StatusCode = RecordStatusCode.Active,
                    Files = new List<FileModel>(),
                };
            }

            var result = await _helpService.GetBugTicketCommentAsync(id);

            return new BugTicketCommentManageModel(result);
        }

        [HttpPost]
        public async Task<Result<BugTicketCommentManageModel>> UpdateBugTicketComment([FromBody]BugTicketCommentManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            if (item != null)
            {
                item.IdEditedBy = Int32.Parse(Request.HttpContext.User.GetUserId());
            }

            item = await _helpService.UpdateBugTicketCommentAsync(item);

            return new BugTicketCommentManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBugTicketComment(int id)
        {
            return await _helpService.DeleteBugTicketCommentAsync(id, Int32.Parse(Request.HttpContext.User.GetUserId()));
        }

        #endregion

        #region BugFilesArea

        [HttpPost]
        public async Task<Result<FileModel>> UploadFile()
        {
            var form = await Request.ReadFormAsync();

            var data = form["data"];

            int? bugTicketId = null;
            int? butTicketCommentId = null;
            string publicId=null;
            string description = String.Empty;
            foreach (string pairs in data)
            {
                if (!String.IsNullOrEmpty(pairs))
                {
                    string[] values = pairs.Split('=');
                    if (values[0] == "bug-ticket-id")
                    {
                        bugTicketId = Int32.Parse(values[1]);
                    }
                    if (values[0] == "bug-ticket-comment-id")
                    {
                        butTicketCommentId = Int32.Parse(values[1]);
                    }
                    if (values[0] == "public-id")
                    {
                        publicId = values[1];
                    }
                    if (values[0] == "description")
                    {
                        description = values[1];
                    }
                }
            }

            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(form.Files[0].ContentDisposition);

            var contentType = form.Files[0].ContentType;
            using (var stream = form.Files[0].OpenReadStream())
            {
                var fileContent = stream.ReadFully();
                try
                {
                    BugFile file = null;
                    var mode = bugTicketId.HasValue ? BugFileType.Ticket : BugFileType.Comment;
                    var fileName = await _helpService.UploadBugFileToStoreAsync(mode, fileContent, parsedContentDisposition.FileName.Replace("\"", ""), publicId, contentType);
                    if(!String.IsNullOrEmpty(fileName) && ((bugTicketId.HasValue && bugTicketId!=0) || (butTicketCommentId.HasValue && butTicketCommentId!=0)))
                    {
                        file = new BugFile
                        {
                            IdBugTicket = bugTicketId,
                            IdBugTicketComment = butTicketCommentId,
                            FileName = fileName,
                            Description = description
                        };

                        file = await _helpService.AddBugFileAsync(file);
                    }

                    var uploadDate = file?.UploadDate ?? DateTime.Now;
                    var id = file?.Id ?? 0;
                    return new FileModel() { FileName = fileName, UploadDate = uploadDate, Id=id };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    throw;
                }
            }
        }

        [HttpGet]
        public async Task<FileResult> GetBugTicketFile([FromQuery]string publicId, [FromQuery]string fileName, [FromQuery]bool viewMode)
        {
            var blob = await _helpService.DownloadBugFileAsync(BugFileType.Ticket, fileName, publicId);

            var contentDisposition = new ContentDispositionHeaderValue(viewMode ? "inline" : "attachment")
            {
                FileName = fileName
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(blob.File, blob.ContentType);
        }

        [HttpGet]
        public async Task<FileResult> GetBugTicketCommentFile([FromQuery]string publicId, [FromQuery]string fileName, [FromQuery]bool viewMode)
        {
            var blob = await _helpService.DownloadBugFileAsync(BugFileType.Comment, fileName, publicId);

            var contentDisposition = new ContentDispositionHeaderValue(viewMode ? "inline" : "attachment")
            {
                FileName = fileName
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(blob.File, blob.ContentType);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBugTicketFile([FromBody]DeleteFileModel model)
        {
            await _helpService.DeleteBugFileFromStoreAsync(BugFileType.Ticket, model.FileName, model.PublicId);
            if (model.Id != 0)
            {
                await _helpService.DeleteBugFileAsync(model.Id);
            }

            return true;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBugTicketCommentFile([FromBody]DeleteFileModel model)
        {
            await _helpService.DeleteBugFileFromStoreAsync(BugFileType.Comment, model.FileName, model.PublicId);
            if (model.Id != 0)
            {
                await _helpService.DeleteBugFileAsync(model.Id);
            }

            return true;
        }

        #endregion
    }
}