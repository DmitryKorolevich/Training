using System;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Profiling.Base
{
    public class RelationalCommandBuilderFacade : IRelationalCommandBuilder
    {
        private readonly IRelationalCommandBuilder _builder;

        public RelationalCommandBuilderFacade(IRelationalCommandBuilder builder)
        {
            _builder = builder;
        }

        public IRelationalCommandBuilder AddParameter(string name, object value, Func<IRelationalTypeMapper, RelationalTypeMapping> mapType,
            bool? nullable)
        {
            return new RelationalCommandBuilderFacade(_builder.AddParameter(name, value, mapType, nullable));
        }

        public IRelationalCommand BuildRelationalCommand()
        {
            var command = _builder.BuildRelationalCommand();
            if (command is RelationalCommandFacade)
                return command;
            return new RelationalCommandFacade(command);
        }

        public IndentedStringBuilder CommandTextBuilder => _builder.CommandTextBuilder;
    }
}