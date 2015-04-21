﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;

namespace VitalChoice.Data.Services
{
    public interface IGenericService<TEntity> where TEntity : Entity
	{
        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        bool InsertRange(IEnumerable<TEntity> entities);
	    Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities);
        TEntity InsertGraph(TEntity entity);
		Task<TEntity> InsertGraphAsync(TEntity entity);
	    bool InsertGraphRange(params TEntity[] entities);
	    Task<bool> InsertGraphRangeAsync(params TEntity[] entities);
	    TEntity Update(TEntity entity);
	    Task<TEntity> UpdateAsync(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entity);
		IEnumerable<TEntity> Query();
		IEnumerable<TEntity> Query(IQueryObject<TEntity> queryObject);
		IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> query);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, int id);
    }
}