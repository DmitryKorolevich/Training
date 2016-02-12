using System;
using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.Context;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Data.Transaction
{
    public interface ITransactionAccessor<TContext> 
        where TContext : IDataContextAsync
    {
        IInnerEmbeddingTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted);
        IUnitOfWorkAsync CreateUnitOfWork();
    }

    public class TransactionAccessor<TContext> : ITransactionAccessor<TContext> where TContext : IDataContextAsync
    {
        private readonly IOptions<AppOptionsBase> _appOptions;
        private readonly TContext _context;
        private IInnerEmbeddingTransaction _transaction;

        protected TransactionAccessor(IOptions<AppOptionsBase> appOptions, TContext context)
        {
            _appOptions = appOptions;
            _context = context;
        }

        public IInnerEmbeddingTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
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
            DataContext context = (DataContext) Activator.CreateInstance(typeof (TContext), _appOptions);
            return new UnitOfWorkBase(context);
        }
    }
}