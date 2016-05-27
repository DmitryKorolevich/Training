using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace VitalChoice.Profiling.Base
{
    public class RelationalCommandBuilderFacade : IRelationalCommandBuilder
    {
        private readonly IRelationalCommandBuilder _builder;

        public RelationalCommandBuilderFacade(IRelationalCommandBuilder builder)
        {
            _builder = builder;
        }

        public IndentedStringBuilder Instance => _builder.Instance;
        public IRelationalCommand Build()
        {
            var command = _builder.Build();
            if (command is RelationalCommandFacade)
                return command;
            return new RelationalCommandFacade(command);
        }

        public IRelationalParameterBuilder ParameterBuilder => _builder.ParameterBuilder;
    }
}