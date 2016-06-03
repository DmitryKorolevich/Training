using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.VeraCore;

namespace VitalChoice.Interfaces.Services.VeraCore
{
    public interface IVeraCoreFilesCacheService
    {
        FileStream GetCached(VeraCoreFileInfo fileInfo, string subPath);

        bool CacheFile(VeraCoreFileInfo file, Stream fileData, string subPath);

        void MakeBackup();
    }
}
