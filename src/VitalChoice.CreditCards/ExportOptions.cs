using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.CreditCards
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

    public class VeraCoreAuth
    {
        public string Password { get; set; }

        public string UserName { get; set; }
    }

    public class ExportOptions : AppOptions
    {
        public int ScheduleDayTimeHour { get; set; }
        public ExportDbConnection ExportConnection { get; set; }

        public VeraCoreAuth VeraCore { get; set; }
    }
}
