using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.ExportService.Context
{
    public class ExportDbConnection
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Encrypt { get; set; }

        public string AdminUserName { get; set; }
        public string AdminPassword { get; set; }
    }

    public class ExportOptions : AppOptions
    {
        public ExportDbConnection ExportConnection { get; set; }
    }
}
