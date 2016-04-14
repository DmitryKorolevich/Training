using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;

namespace VitalChoice.Profiling.Base
{
    public class RelationalCommandBuilderFactoryProxy : RelationalCommandBuilderFactory
    {
        public RelationalCommandBuilderFactoryProxy(ISensitiveDataLogger<RelationalCommandBuilderFactory> logger,
            DiagnosticSource diagnosticSource, IRelationalTypeMapper typeMapper) : base(logger, diagnosticSource, typeMapper)
        {
        }

        public override IRelationalCommandBuilder Create()
        {
            return new RelationalCommandBuilderFacade(base.Create());
        }
    }
}