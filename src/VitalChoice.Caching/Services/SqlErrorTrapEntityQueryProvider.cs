using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using VitalChoice.Caching.Iterators;

namespace VitalChoice.Caching.Services
{
    public class SqlErrorTrapEntityQueryProvider : EntityQueryProvider, IAsyncQueryProviderBase
    {
        public SqlErrorTrapEntityQueryProvider(IQueryCompiler queryCompiler) : base(queryCompiler)
        {
        }

        public override object Execute(Expression expression)
        {
            return Execute<object>(expression);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            SqlErrorTrapHandler<TResult, Expression> trapHandler =
                new SqlErrorTrapHandler<TResult, Expression>(e => base.Execute<TResult>(e), expression);
            return trapHandler.GetResult();
        }

        public IAsyncEnumerable<TResult> ExecuteBaseAsync<TResult>(Expression expression)
        {
            return base.ExecuteAsync<TResult>(expression);
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new AsyncSqlErrorTrapEnumerable<TResult>(base.ExecuteAsync<TResult>(expression), this,
                expression);
        }

        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            AsyncSqlErrorTrapHandler<TResult, Expression> trapHandler =
                new AsyncSqlErrorTrapHandler<TResult, Expression>((e, c) => base.ExecuteAsync<TResult>(e, c), expression);
            return trapHandler.GetResult(cancellationToken);
        }
    }
}