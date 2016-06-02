using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class ProMailSettings
    {
	    public string ExportFolderName { get; set; }
        public string ServerHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ServerPort { get; set; }
    }
}
