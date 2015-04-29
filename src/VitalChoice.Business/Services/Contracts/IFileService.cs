﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Files;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Contracts
{
	public interface IFileService
    {
        DirectoryInfoObject GetDirectories();

        DirectoryInfoObject AddDirectory(string fullRelativeName, string name);

        bool DeleteDirectory(string fullRelativeName);

        IEnumerable<FileInfoObject> GetFiles(string fullRelativeName);

        FileInfoObject AddFile(string fullRelativeName, string name, byte[] content);

        bool DeleteFile(string fullRelativeName);
    }
}
