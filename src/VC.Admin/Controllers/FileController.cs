using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Settings;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using System;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Business.Services;
using VitalChoice.Core.Base;
using VitalChoice.Domain.Entities.Files;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Interfaces.Services;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Core.Services;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Files)]
    public class FileController : BaseApiController
    {
        private readonly IFileService fileService;
        private readonly ILogger logger;

        public FileController(IFileService fileService,
            ILoggerProviderExtended loggerProvider)
        {
            this.fileService = fileService;
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpGet]
        public Result<DirectoryInfoObject> GetDirectories()
        {
            return fileService.GetDirectories();
        }

        [HttpPost]
        public Result<DirectoryInfoObject> AddDirectory([FromBody]DirectoryInfoObject model)
        {
            return fileService.AddDirectory(model.FullRelativeName, model.Name);
        }

        [HttpPost]
        public Result<bool> DeleteDirectory([FromBody]DirectoryInfoObject model)
        {
            return fileService.DeleteDirectory(model.FullRelativeName);
        }

        [HttpPost]
        public Result<IEnumerable<FileInfoObject>> GetFiles([FromBody]DirectoryInfoObject model)
        {
            return fileService.GetFiles(model.FullRelativeName).ToList();
        }


        [HttpPost]
        public async Task<Result<FileInfoObject>> AddFiles()
        {
            FileInfoObject toReturn = null;
            var form = await Request.ReadFormAsync();

            var fullRelativeUrl = form.Keys.FirstOrDefault();

            List<MessageInfo> messages = new List<MessageInfo>();
            foreach (var file in form.Files)
            {
                var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                using (var stream = file.OpenReadStream())
                {
                    var fileContent = stream.ReadFully();
                    try
                    {
                        toReturn=fileService.AddFile(fullRelativeUrl, parsedContentDisposition.FileName.Replace("\"", ""), fileContent);
                    }
                    catch (AppValidationException ex)
                    {
                        messages.AddRange(ex.Messages);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex.ToString());
                    }
                }
            }

            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            return toReturn;
        }

        [HttpPost]
        public Result<bool> DeleteFile([FromBody]FileInfoObject model)
        {
            return fileService.DeleteFile(model.FullRelativeName);
        }
    }
}