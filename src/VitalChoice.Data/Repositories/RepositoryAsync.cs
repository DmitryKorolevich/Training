﻿using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Repositories
{
	public class RepositoryAsync<TEntity> : ReadRepositoryAsync<TEntity>, IRepositoryAsync<TEntity> where TEntity : Entity
	{
		public RepositoryAsync(IDataContextAsync context) : base(context)
		{
		}

		public virtual void Insert(TEntity entity)
		{
			DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Added);
			Context.SaveChanges();
		}

		public virtual void InsertRange(IEnumerable<TEntity> entities)
		{
			foreach (var entity in entities)
				Insert(entity);
			Context.SaveChanges();
		}

		public virtual void InsertGraph(TEntity entity)
		{
			DbSet.Add(entity);
			Context.SaveChanges();
		}

		public virtual void InsertGraphRange(params TEntity[] entities)
		{
			DbSet.Add(entities);
			Context.SaveChanges();
		}

		public virtual void Update(TEntity entity)
		{
			DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Modified);
			Context.SaveChanges();
		}

		public virtual void Delete(int id)
		{
			var entity = DbSet.FirstOrDefault(x=>x.Id == id);
			if (entity!= null)
			{
				Delete(entity);
			}
		}

		public virtual void Delete(TEntity entity)
		{
			DbSet.Attach(entity);
			Context.SetState(entity, EntityState.Deleted);
			Context.SaveChanges();
		}

		public virtual async Task<bool> DeleteAsync(int id)
		{
			return await DeleteAsync(CancellationToken.None, id);
		}

		public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, int id)
		{
			var entity = await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
			if (entity == null) return false;

			DbSet.Attach(entity);
			DbSet.Remove(entity);
			Context.SaveChanges();
			return true;
		}
	}
}
