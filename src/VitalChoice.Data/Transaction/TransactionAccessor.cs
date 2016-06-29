using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Context;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Data.Transaction
{
    public interface ITransactionAccessor<TContext> 
        where TContext : DbContext, IDataContextAsync
    {
        IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted);
        IUnitOfWorkAsync CreateUnitOfWork();
    }

    public class TransactionAccessor<TContext> : ITransactionAccessor<TContext> 
        where TContext : DbContext, IDataContextAsync
    {
        private readonly IOptions<AppOptionsBase> _appOptions;
        private readonly DbContextOptions<TContext> _contextOptions;
        private readonly TContext _context;
        private IScopedTransaction _transaction;

        public TransactionAccessor(IOptions<AppOptionsBase> appOptions, DbContextOptions<TContext> contextOptions, TContext context)
        {
            _appOptions = appOptions;
            _contextOptions = contextOptions;
            _context = context;
        }

        public IScopedTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
        {
            if (_transaction == null || _transaction.Closed)
            {
                _transaction = _context.BeginTransaction();
                return _transaction;
            }
            return _transaction.DbContext.BeginTransaction();
        }

        public IUnitOfWorkAsync CreateUnitOfWork()
        {
            if (_transaction != null && !_transaction.Closed)
            {
                return new UnitOfWorkBase(_transaction.DbContext);
            }
            var context = (IDataContextAsync) Activator.CreateInstance(typeof (TContext), _appOptions, _contextOptions);
            return new UnitOfWorkBase(context);
        }
    }
}