using System;
using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.DataContext;

namespace VitalChoice.Data.Transaction
{
    public class TransactionAccessor
    {
	    private readonly DbContext _dataContext;

	    public TransactionAccessor(IDataContextAsync dataContext)
	    {
		    this._dataContext = (DbContext)dataContext;
	    }

	    public IRelationalTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted)
	    {
			return _dataContext.Database.BeginTransaction(isolation);
		}
    }
}