using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace VitalChoice.Profiling.Base
{
    public class QuerySqlGeneratorProxy : IQuerySqlGenerator
    {
        private readonly IQuerySqlGenerator _sqlGenerator;

        public QuerySqlGeneratorProxy(IQuerySqlGenerator sqlGenerator)
        {
            _sqlGenerator = sqlGenerator;
        }

        public IRelationalCommand GenerateSql(IReadOnlyDictionary<string, object> parameterValues)
        {
            var command = _sqlGenerator.GenerateSql(parameterValues);
            if (command is RelationalCommandFacade)
            {
                return command;
            }
            return new RelationalCommandFacade(command);
        }

        public bool IsCacheable => _sqlGenerator.IsCacheable;

        public IRelationalValueBufferFactory CreateValueBufferFactory(IRelationalValueBufferFactoryFactory relationalValueBufferFactoryFactory,
            DbDataReader dataReader)
        {
            return _sqlGenerator.CreateValueBufferFactory(relationalValueBufferFactoryFactory, dataReader);
        }
    }
}
