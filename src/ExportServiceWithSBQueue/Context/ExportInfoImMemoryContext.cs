using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace VitalChoice.ExportService.Context
{
    public class ExportInfoImMemoryContext : ExportInfoContext
    {
        public ExportInfoImMemoryContext(IOptions<ExportOptions> options, DbContextOptions<ExportInfoContext> contextOptions)
            : base(options, contextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseInMemoryDatabase(options => options.IgnoreTransactions());
        }
    }
}