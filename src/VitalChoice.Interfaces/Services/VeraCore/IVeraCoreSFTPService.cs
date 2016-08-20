using System.Collections.Generic;
using System.IO;
using VitalChoice.Ecommerce.Domain.Entities.VeraCore;

namespace VitalChoice.Interfaces.Services.VeraCore
{
    public interface IVeraCoreSFTPService
    {
        string GetWorkingDirectory();

        void UploadFile(VeraCoreSFTPOptions options, Stream file, string name);

        ICollection<VeraCoreFileInfo> GetFileList(VeraCoreSFTPOptions options);

        void RemoveFile(string fileName);

        MemoryStream DownloadFileData(string fileName);
    }
}
