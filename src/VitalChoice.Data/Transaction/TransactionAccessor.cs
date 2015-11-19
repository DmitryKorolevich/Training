using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.Context;

namespace VitalChoice.Data.Transaction
{
    public class TransactionAccessor
    {
	    private readonly IDataContext _dataContext;

	    public TransactionAccessor(IDataContext dataContext)
	    {
		    _dataContext = dataContext;
	    }

	    public IRelationalTransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadUncommitted)
	    {
			return _dataContext.BeginTransaction(isolation);
		}
    }
}