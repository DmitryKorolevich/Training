using System.Collections.Generic;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Storage.Internal;

namespace VitalChoice.Profiling.Base
{
    public class SqlCommandBuilderProxy : SqlCommandBuilder
    {
        public SqlCommandBuilderProxy(IRelationalCommandBuilderFactory commandBuilderFactory, ISqlGenerator sqlGenerator,
            IParameterNameGeneratorFactory parameterNameGeneratorFactory)
            : base(commandBuilderFactory, sqlGenerator, parameterNameGeneratorFactory)
        {
        }

        public override IRelationalCommand Build(string sql, IReadOnlyList<object> parameters = null)
        {
            var command = base.Build(sql, parameters);
            if (command is RelationalCommandFacade)
                return command;
            return new RelationalCommandFacade(command);
        }
    }
}