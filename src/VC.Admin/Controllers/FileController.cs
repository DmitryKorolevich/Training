using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Models.Setting;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Models;
using VitalChoice.Business.Services.Contracts.Settings;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.Settings;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using System.IO;
using System;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Business.Helpers;

namespace VitalChoice.Controllers
{
    public class FileController : BaseApiController
    {
        private readonly IFileService fileService;
        private readonly ILogger logger;

        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
            this.logger = LoggerService.GetDefault();
        }

        [HttpPost]
        public async Task<Result<bool>> AddFiles()
        {
            var form = await Request.ReadFormAsync();

            var fullRelativeUrl = form.Keys.FirstOrDefault();

            List<MessageInfo> messages = new List<MessageInfo>();
            foreach (var file in form.Files)
            {
                var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                using (var stream = file.OpenReadStream())
                {
                    var fileContent = StreamsHelper.ReadFully(stream);
                    try
                    {
                        fileService.AddFile(fullRelativeUrl, parsedContentDisposition.FileName.Replace("\"", ""), fileContent);
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

            return true;
        }        
    }
}