using System;
using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Relational;
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

	    public RelationalTransaction BeginTransaction()
	    {
			return _dataContext.Database.AsRelational().AsSqlServer().Connection.BeginTransaction(IsolationLevel.ReadCommitted);
		}
    }
}