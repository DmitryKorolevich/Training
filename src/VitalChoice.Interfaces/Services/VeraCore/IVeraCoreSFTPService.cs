using System.Collections.Generic;
using System.IO;
using VitalChoice.Ecommerce.Domain.Entities.VeraCore;

namespace VitalChoice.Interfaces.Services.VeraCore
{
    public interface IVeraCoreSFTPService
    {
        string WorkingDirectory { get; }

        ICollection<VeraCoreFileInfo> GetFileList(VeraCoreSFTPOptions options);

        void RemoveFile(string fileName);

        MemoryStream DownloadFileData(string fileName);
    }
}
