using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Controllers;
using VitalChoice.Validation.Exceptions;
using VitalChoice.Validation.Helpers;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace QRProject.Api.Controllers.Base
{
    public abstract class BaseApiController: Controller
    {
        //protected static readonly Logger SLog = LkLoggerFactory.GetDefault();
        //protected static readonly PijnzControllerClient PijnzIntegrationWcfClient = QRIntegrationHelper.GetQRIntegrationsServiceClient();

        protected BaseApiController()
        {
            Settings = ControllerSettings.Create();
            Parse(GetType(), Settings);
        }

        [NonAction]
        public T ConvertWithValidate<T, TViewMode>(Model<T, TViewMode> model, T dbEntity = null)
            where T: class, new()
            where TViewMode: class, IMode
        {
            string actionName = ControllerContext.Request.Properties["actionName"] as string;

            model.Mode = Settings.GetValidationMode<TViewMode>(actionName);
            model.Validate();
            if (!model.IsValid) {
                foreach (var validationError in model.Errors) {
                    ModelState.AddModelError(validationError.Key, validationError.Value);
                }
            }
            else {
                return dbEntity == null ? model.Convert() : model.Update(dbEntity);
            }
            return null;
        }

        [HttpGet]
        public Result<ControllerSettings> GetSettings()
        {
            return Settings;
        }

        [NonAction]
        public static Task<Result<FileTP>> AddFile(HttpRequestMessage request, Func<FileData, FileTypeTP, FileTP> addFileFucn, Func<FileData, FileTypeTP, bool> fileCheckFunc)
        {
            if (request.Content.IsMimeMultipartContent()) {
                MultipartMemoryStreamProvider provider = new MultipartMemoryStreamProvider();
                return request.Content.ReadAsMultipartAsync(provider).ContinueWith<Result<FileTP>>
                    (t =>
                     {
                         if (t.IsFaulted || t.IsCanceled)
                             throw new ArgumentException();

                         FileData fileData = null;
                         FileTypeTP? type = null;
                         MultipartMemoryStreamProvider streamProvider = t.Result;
                         foreach (HttpContent content in streamProvider.Contents) {
                             if (!String.IsNullOrEmpty(content.Headers.ContentDisposition.FileName)) {
                                 byte[] file = StreamsHelper.ReadFully(content.ReadAsStreamAsync().Result);
                                 fileData = new FileData
                                            {
                                                FileName =
                                                    content.Headers.ContentDisposition.FileName.Substring
                                                    (1,
                                                     content.Headers.ContentDisposition.FileName.Length - 2),
                                                Data = file
                                            };
                             }
                             else {
                                 string param = content.ReadAsStringAsync().Result;
                                 type = (FileTypeTP)Enum.Parse(typeof(FileTypeTP), param, true);
                             }
                         }

                         if (fileData != null && fileData.Data.Length != 0 && type.HasValue) {
                             if (!fileCheckFunc(fileData, type.Value)) {
                                 throw new ArgumentException();
                             }
                             return addFileFucn(fileData, type.Value);
                         }
                         throw new ApiValidationException("Api.Exception.App.FileNotSpecified");
                     });
            }
            throw new ArgumentException();
        }

        [NonAction]
        internal abstract void Configure(string actionName);

        public ControllerSettings Settings { get; private set; }

        [NonAction]
        protected void Parse(Type controllerType, ControllerSettings settings)
        {
            var markedMethods = controllerType.GetTypeInfo().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(ControlModeAttribute), false).Any());
            foreach (var markedMethod in markedMethods) {
                var controlMode =
                    markedMethod.GetCustomAttributes(typeof(ControlModeAttribute), false).SingleOrDefault() as
                    ControlModeAttribute;
                if (controlMode != null) {
                    settings.SetMode(controlMode.ViewModeType, controlMode.Mode, markedMethod.Name);
                }
            }
        }
    }
}