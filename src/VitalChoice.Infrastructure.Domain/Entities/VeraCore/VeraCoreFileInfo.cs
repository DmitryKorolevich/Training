using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.VeraCore
{
    public class VeraCoreFileInfo
    {
        public string FileName{get;set;}

        public DateTime FileDate{get;set;}

        public long FileSize{get;set;}
    }
}
