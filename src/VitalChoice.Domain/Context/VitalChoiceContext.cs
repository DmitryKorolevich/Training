using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Framework.ConfigurationModel;
using VitalChoice.Data;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Infrastructure;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Domain.Context
{
	public class VitalChoiceContext : IdentityDbContext<AppUser>, IDataContext, IDataContextAsync
	{
		private readonly Guid instanceId;
/*
		public VitalChoiceContext(DbContextOptions contextOptions)
				: base(nameOrConnectionString)
		{
			instanceId = Guid.NewGuid();
			Configuration.LazyLoadingEnabled = false;
			Configuration.ProxyCreationEnabled = false;
		}*/

		public Guid InstanceId
		{
			get { return instanceId; }
		}

		public DbSet<T> Set<T>() where T : class
		{
			return base.Set<T>();
		}

		protected override void OnConfiguring(DbContextOptions builder)
		{
			builder.UseSqlServer(@"Server=(localdb)\v11.0;Database=Blogging;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Blog>()
				.OneToMany(b => b.Posts, p => p.Blog)
				.ForeignKey(p => p.BlogId);
		}

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

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
