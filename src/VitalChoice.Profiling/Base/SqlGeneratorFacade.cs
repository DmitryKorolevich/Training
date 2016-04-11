using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Storage;

namespace VitalChoice.Profiling.Base
{
    public class SqlGeneratorFacade : ISqlQueryGenerator
    {
        private readonly ISqlQueryGenerator _sqlQueryGenerator;

        public SqlGeneratorFacade(ISqlQueryGenerator sqlQueryGenerator)
        {
            _sqlQueryGenerator = sqlQueryGenerator;
        }

        public IRelationalCommand GenerateSql(IDictionary<string, object> parameterValues)
        {
            var command = _sqlQueryGenerator.GenerateSql(parameterValues);
            return new RelationalCommandFacade(command);
        }

        public IRelationalValueBufferFactory CreateValueBufferFactory(
            IRelationalValueBufferFactoryFactory relationalValueBufferFactoryFactory,
            DbDataReader dataReader)
        {
            return _sqlQueryGenerator.CreateValueBufferFactory(relationalValueBufferFactoryFactory, dataReader);
        }
    }
}