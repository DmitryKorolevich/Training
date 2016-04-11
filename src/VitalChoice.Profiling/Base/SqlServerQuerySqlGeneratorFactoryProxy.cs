using System;
using System.Linq;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Query.Expressions;
using Microsoft.Data.Entity.Query.Sql;
using Microsoft.Data.Entity.Query.Sql.Internal;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Update;
using Microsoft.Data.Entity.Update.Internal;

namespace VitalChoice.Profiling.Base
{
    public class SqlServerQuerySqlGeneratorFactoryProxy : SqlServerQuerySqlGeneratorFactory
    {
        public SqlServerQuerySqlGeneratorFactoryProxy(IRelationalCommandBuilderFactory commandBuilderFactory, ISqlGenerator sqlGenerator,
            IParameterNameGeneratorFactory parameterNameGeneratorFactory, ISqlCommandBuilder sqlCommandBuilder)
            : base(commandBuilderFactory, sqlGenerator, parameterNameGeneratorFactory, sqlCommandBuilder)
        {
        }

        public override ISqlQueryGenerator CreateGenerator(SelectExpression selectExpression)
        {
            var generator = base.CreateGenerator(selectExpression);
            return new SqlGeneratorFacade(generator);
        }

        public override ISqlQueryGenerator CreateRawCommandGenerator(SelectExpression selectExpression, string sql, object[] parameters)
        {
            var generator = base.CreateRawCommandGenerator(selectExpression, sql, parameters);
            return new SqlGeneratorFacade(generator);
        }
    }
}