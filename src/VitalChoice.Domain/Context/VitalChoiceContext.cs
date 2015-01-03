using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Framework.ConfigurationModel;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Mapping;

namespace VitalChoice.Domain.Context
{
	public class VitalChoiceContext : IdentityDbContext<ApplicationUser>, IDataContext, IDataContextAsync
	{
		private readonly Guid instanceId;

	public VitalChoiceContext(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
		instanceId = Guid.NewGuid();
		Configuration.LazyLoadingEnabled = false;
		Configuration.ProxyCreationEnabled = false;
	}

	public Guid InstanceId
	{
		get { return instanceId; }
	}

	public new DbSet<T> Set<T>() where T : class
	{
		return base.Set<T>();
	}

	/*protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}*/

	public override int SaveChanges()
	{
		SyncObjectsStatePreCommit();
		var changes = base.SaveChanges();
		SyncObjectsStatePostCommit();
		return changes;
	}

	public override async Task<int> SaveChangesAsync()
	{
		SyncObjectsStatePreCommit();
		var changesAsync = await base.SaveChangesAsync();
		SyncObjectsStatePostCommit();
		return changesAsync;
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
	{
		SyncObjectsStatePreCommit();
		var changesAsync = await base.SaveChangesAsync(cancellationToken);
		SyncObjectsStatePostCommit();
		return changesAsync;
	}

	public void SyncObjectState(object entity)
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
	}
}
}
