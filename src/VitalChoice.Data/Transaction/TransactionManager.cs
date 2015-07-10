using System;
using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.DataContext;

namespace VitalChoice.Data.Transaction
{
    public class TransactionManager
    {
	    private readonly DbContext _dataContext;

	    public TransactionManager(IDataContextAsync dataContext)
	    {
		    this._dataContext = (DbContext)dataContext;
	    }

	    public IRelationalTransaction BeginTransaction()
	    {
			return _dataContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);
		}

        public IRelationalTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _dataContext.Database.BeginTransaction(isolationLevel);
        }
    }
}