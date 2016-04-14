using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Profiling.Base
{
    public class CommandBuilderProxy : CommandBuilder
    {
        public CommandBuilderProxy(IRelationalValueBufferFactoryFactory valueBufferFactoryFactory,
            Func<ISqlQueryGenerator> sqlGeneratorFunc) : base(valueBufferFactoryFactory, sqlGeneratorFunc)
        {
        }

        public override IRelationalCommand Build(IDictionary<string, object> parameterValues)
        {
            var command = base.Build(parameterValues);
            if (command is RelationalCommandFacade)
                return command;
            return new RelationalCommandFacade(command);
        }
    }
}