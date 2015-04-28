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

namespace VitalChoice.Controllers
{
    public class FileController : BaseApiController
    {

        private readonly ILogger logger;

        public FileController()
        {
            this.logger = LoggerService.GetDefault();
        }

        [HttpPost]
        public Task<bool> UploadMultiple()
        {
            //var fileDetailsList = new List<FileDetails>();
            return Request.ReadFormAsync().ContinueWith<bool>(t=>
                {
                    if (t.IsFaulted || t.IsCanceled)
                        throw new ArgumentException();
                    
                    var form = t.Result;

                    foreach (var file in form.Files)
                    {
                        var parsedContentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                        using (var reader = new StreamReader(file.OpenReadStream()))
                        {
                            var fileContent = reader.ReadToEnd();
                            var fileDetails = new
                            {
                                Filename = parsedContentDisposition.FileName,
                                Content = fileContent
                            };
                        }
                    }

                    return true;
                });
        }
    }
}