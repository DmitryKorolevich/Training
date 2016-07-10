using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VC.Admin.Models;
using VitalChoice.Validation.Models;
using System;
using System.IO;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Help;
using VC.Admin.Models.Help;
using Microsoft.Net.Http.Headers;
using VitalChoice.Core.Infrastructure.Helpers;
using Microsoft.Extensions.Primitives;
using VitalChoice.SharedWeb.Models.Help;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer.Help;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Help)]
    public class HelpController : BaseApiController
    {
        private readonly IHelpService _helpService;
        private readonly ExtendedUserManager _userManager;
        private readonly ReferenceData _referenceData;
        private readonly ILogger _logger;

        public HelpController(IHelpService helpService,
            ILoggerProviderExtended loggerProvider, ExtendedUserManager userManager, ReferenceData referenceData)
        {
            _helpService = helpService;
            _userManager = userManager;
            _referenceData = referenceData;
            _logger = loggerProvider.CreateLogger<HelpController>();
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
                    Priority = TicketPriority.Low,
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

            item = await _helpService.UpdateHelpTicketAsync(item, Int32.Parse(_userManager.GetUserId(User)));

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
                item.IdEditedBy = Int32.Parse(_userManager.GetUserId(User));
            }

            item = await _helpService.UpdateHelpTicketCommentAsync(item);

            return new HelpTicketCommentManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteHelpTicketComment(int id)
        {
            return await _helpService.DeleteHelpTicketCommentAsync(id, Int32.Parse(_userManager.GetUserId(User)));
        }

        #endregion

        #region BugsArea

        [HttpPost]
        public async Task<Result<PagedList<BugTicketListItemModel>>> GetBugTickets([FromBody]BugTicketFilter filter)
        {
            if (filter.To.HasValue)
                filter.To = filter.To.Value.AddDays(1);
            var result = await _helpService.GetBugTicketsAsync(filter);

            var toReturn = new PagedList<BugTicketListItemModel>
            {
                Items = result.Items.Select(p => new BugTicketListItemModel(p)).ToList(),
                Count = result.Count,
            };

            var superAdmin = _referenceData.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdmin.Normalize());
            int userId = Int32.Parse(_userManager.GetUserId(User));
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
                    StatusCode = BugTicketStatus.Active,
                    Priority = TicketPriority.Low,
                    IsAllowEdit=true,
                    Comments = new List<BugTicketCommentManageModel>(),
                    Files = new List<FileModel>(),
                };
            }

            var result = await _helpService.GetBugTicketAsync(id);
            var toReturn= new BugTicketManageModel(result);

            var superAdmin = _referenceData.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            if(HttpContext.User.IsInRole(superAdmin.Normalize()) || result.IdAddedBy== Int32.Parse(_userManager.GetUserId(User)))
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

            var superAdmin = _referenceData.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdmin.Normalize());
            item = await _helpService.UpdateBugTicketAsync(item, Int32.Parse(_userManager.GetUserId(User)), isSuperAdmin);

            return new BugTicketManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBugTicket(int id)
        {
            var superAdmin = _referenceData.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdmin.Normalize());

            return await _helpService.DeleteBugTicketAsync(id, Int32.Parse(_userManager.GetUserId(User)), isSuperAdmin ? (int?)null : Int32.Parse(_userManager.GetUserId(User)));
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
                item.IdEditedBy = Int32.Parse(_userManager.GetUserId(User));
            }

            item = await _helpService.UpdateBugTicketCommentAsync(item);

            return new BugTicketCommentManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBugTicketComment(int id)
        {
            return await _helpService.DeleteBugTicketCommentAsync(id, Int32.Parse(_userManager.GetUserId(User)));
        }

        #endregion

        #region BugFilesArea

        [HttpPost]
        public async Task<Result<FileModel>> UploadFile()
        {
            var form = await Request.ReadFormAsync();

            int? bugTicketId = null;
            int? bugTicketCommentId = null;
            string publicId=null;
            string description = String.Empty;
            if (!StringValues.IsNullOrEmpty(form["bugTicketId"]))
            {
                bugTicketId = Int32.Parse(form["bugTicketId"]);
            }
            if (!StringValues.IsNullOrEmpty(form["bugTicketCommentId"]))
            {
                bugTicketCommentId = Int32.Parse(form["bugTicketCommentId"]);
            }
            if (!StringValues.IsNullOrEmpty(form["publicId"]))
            {
                publicId = form["publicId"];
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
                    if(!String.IsNullOrEmpty(fileName) && ((bugTicketId.HasValue && bugTicketId!=0) || (bugTicketCommentId.HasValue && bugTicketCommentId != 0)))
                    {
                        file = new BugFile
                        {
                            IdBugTicket = bugTicketId,
                            IdBugTicketComment = bugTicketCommentId,
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