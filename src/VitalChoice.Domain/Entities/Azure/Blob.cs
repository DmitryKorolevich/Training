using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Azure
{
    public class Blob
    {
	    public byte[] File { get; set; }

	    public string ContentType { get; set; }
    }
}
