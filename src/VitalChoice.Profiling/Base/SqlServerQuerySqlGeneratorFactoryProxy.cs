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
    public class SqlServerQuerySqlGeneratorFactoryProxy : SqlServerQuerySqlGeneratorFactory
    {
        public SqlServerQuerySqlGeneratorFactoryProxy(IRelationalCommandBuilderFactory commandBuilderFactory,
            ISqlGenerationHelper sqlGenerationHelper, IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            IRelationalTypeMapper relationalTypeMapper)
            : base(commandBuilderFactory, sqlGenerationHelper, parameterNameGeneratorFactory, relationalTypeMapper)
        {
        }

        public override IQuerySqlGenerator CreateDefault(SelectExpression selectExpression)
        {
            var sqlGenerator = base.CreateDefault(selectExpression);
            return new QuerySqlGeneratorProxy(sqlGenerator);
        }
    }
}