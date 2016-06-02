using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Entities.ProMail
{
    public class ProMailFile : ProMailFileInfo
    {
        public byte[] Data { get; set; }
    }
}
