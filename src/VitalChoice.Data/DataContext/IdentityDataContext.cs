using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Data.DataContext
{
	public class IdentityDataContext : IdentityDbContext<ApplicationUser, IdentityRole<int>,int>, IDataContext, IDataContextAsync
	{
		private readonly Guid instanceId;

		public IdentityDataContext()
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
		    base.Entry(entity).State = state;
        }

        public void TrackGraphForAdd(object entity)
        {
            this.ChangeTracker.TrackGraph(entity, e => e.State = EntityState.Added);
        }
    }
}
