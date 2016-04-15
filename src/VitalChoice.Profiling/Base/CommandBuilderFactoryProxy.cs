using System;
using Microsoft.Data.Entity.Query.Internal;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Profiling.Base
{
    public class CommandBuilderFactoryProxy : CommandBuilderFactory
    {
        private readonly IRelationalValueBufferFactoryFactory _valueBufferFactoryFactory;

        public CommandBuilderFactoryProxy(IRelationalValueBufferFactoryFactory valueBufferFactoryFactory) : base(valueBufferFactoryFactory)
        {
            _valueBufferFactoryFactory = valueBufferFactoryFactory;
        }

        public override CommandBuilder Create(Func<ISqlQueryGenerator> sqlGeneratorFunc)
        {
            return new CommandBuilderProxy(_valueBufferFactoryFactory, sqlGeneratorFunc);
        }
    }
}