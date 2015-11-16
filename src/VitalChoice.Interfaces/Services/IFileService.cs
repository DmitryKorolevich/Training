using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Entities.Files;

namespace VitalChoice.Interfaces.Services
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
