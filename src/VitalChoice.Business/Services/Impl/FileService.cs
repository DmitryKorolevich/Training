using Microsoft.Framework.Runtime;
using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Log;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Impl
{
    public class FileService : IFileService
    {
        private static string _rootDir;

        public static void Init(string rootDir)
        {
            if (!String.IsNullOrEmpty(rootDir))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(rootDir);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                    _rootDir = dirInfo.FullName;
                }
            }
        }

        public FileService()
        {
        }

        public bool GetDirectories()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_rootDir);
            var dirs = dirInfo.GetDirectories("*", SearchOption.AllDirectories);

            return false;
        }
    }
}
