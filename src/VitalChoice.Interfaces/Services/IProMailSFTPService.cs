using System;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.IO;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.ProMail;

namespace VitalChoice.Interfaces.Services
{
    public interface IProMailSFTPService
    {
        ICollection<ProMailFileInfo> GetFileList(ProMailSFTPOptions options);

        void RemoveFile(string fileName);

        MemoryStream DownloadFileData(string fileName);
    }
}
