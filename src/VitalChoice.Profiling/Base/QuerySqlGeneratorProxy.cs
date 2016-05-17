using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Query.Sql.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace VitalChoice.Profiling.Base
{
    public class QuerySqlGeneratorProxy : SqlServerQuerySqlGenerator
    {
        public QuerySqlGeneratorProxy(IRelationalCommandBuilderFactory relationalCommandBuilderFactory,
            ISqlGenerationHelper sqlGenerationHelper, IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            IRelationalTypeMapper relationalTypeMapper, SelectExpression selectExpression)
            : base(
                relationalCommandBuilderFactory, sqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper, selectExpression)
        {
        }

        public override IRelationalCommand GenerateSql(IReadOnlyDictionary<string, object> parameterValues)
        {
            var command = base.GenerateSql(parameterValues);
            if (command is RelationalCommandFacade)
            {
                return command;
            }
            return new RelationalCommandFacade(command);
        }
    }
}
