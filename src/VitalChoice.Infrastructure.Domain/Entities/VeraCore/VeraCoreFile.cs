using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.VeraCore
{
    public class VeraCoreFile : VeraCoreFileInfo
    {
        public string Data { get; set; }

        public VeraCoreFile(VeraCoreFileInfo info, string data)
        {
            FileDate = info.FileDate;
            FileName = info.FileName;
            FileSize = info.FileSize;
            Data = data;
        }
    }
}
