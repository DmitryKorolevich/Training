using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class VeraCoreSettings
    {
	    public string ExportFolderName { get; set; }
        public string GiftListFolderName { get; set; }
        public string ServerHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ServerPort { get; set; }
        public string ArchivePath { get; set; }
        public int WAwarehouseThreshold { get; set; }
    }
}
