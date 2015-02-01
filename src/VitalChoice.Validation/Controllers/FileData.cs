using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QRProject.Api.Controllers.Base
{
    public class FileData
    {
        public string FileName { get; set; }

        public byte[] Data { get; set; }
    }
}