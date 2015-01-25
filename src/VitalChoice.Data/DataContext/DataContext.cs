using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.DataContext
{
	public class DataContext : IdentityDbContext<ApplicationUser>, IDataContext, IDataContextAsync
	{
		private readonly Guid instanceId;

		public DataContext()
		{
			instanceId = Guid.NewGuid();
			/*Configuration.LazyLoadingEnabled = false;
			Configuration.ProxyCreationEnabled = false;*/
		}

		public Guid InstanceId => instanceId;

		public override int SaveChanges()
		{
			//SyncObjectsStatePreCommit();
			var changes = base.SaveChanges();
			//SyncObjectsStatePostCommit();
			return changes;
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
		{
			//SyncObjectsStatePreCommit();
			var changesAsync = await base.SaveChangesAsync(cancellationToken);
			//SyncObjectsStatePostCommit();
			return changesAsync;
		}

		public void SetState(object entity, EntityState state)
		{
			Entry(entity).State = state;
        }

		/*public void SyncObjectState(object entity)
		{
            Entry(entity).State = StateHelper.ConvertState(((IObjectState)entity).ObjectState);
		}

		private void SyncObjectsStatePreCommit()
		{
			foreach (var dbEntityEntry in ChangeTracker.Entries())
				dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
		}

		public void SyncObjectsStatePostCommit()
		{
			foreach (var dbEntityEntry in ChangeTracker.Entries())
				((IObjectState)dbEntityEntry.Entity).ObjectState = StateHelper.ConvertState(dbEntityEntry.State);
		}*/
	}
}
