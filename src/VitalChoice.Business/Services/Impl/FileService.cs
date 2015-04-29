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
        private const string ROOT_DIR = "//Files";

        private readonly IApplicationEnvironment applicationEnvironment;

        public static void Init(string appFolder)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(appFolder + ROOT_DIR);
            if(!dirInfo.Exists)
            {
                dirInfo.Create();
            }
        }

        public FileService(IApplicationEnvironment applicationEnvironment)
        {
            this.applicationEnvironment = applicationEnvironment;
        }

        public bool GetDirectories()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(applicationEnvironment.ApplicationBasePath + ROOT_DIR);
            var dirs = dirInfo.GetDirectories("*", SearchOption.AllDirectories);

            return false;
        }
    }
}
