﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Domain;

namespace VitalChoice.Data.DataContext
{
	public class DataContext : DbContext, IDataContext, IDataContextAsync
	{
		private readonly Guid instanceId;

		public DataContext()
		{
			instanceId = Guid.NewGuid();
        }

		public Guid InstanceId => instanceId;

		public override int SaveChanges()
		{
			var changes = base.SaveChanges();
			return changes;
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			var changesAsync = await base.SaveChangesAsync(cancellationToken);
			return changesAsync;
		}

		public void SetState(object entity, EntityState state)
		{
            SetState(entity, state);
        }
	}
}
