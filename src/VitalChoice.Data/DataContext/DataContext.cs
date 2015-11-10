using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace VitalChoice.Data.DataContext
{
	public class DataContext : DbContext, IDataContext, IDataContextAsync
	{
	    public DataContext()
		{
			InstanceId = Guid.NewGuid();
        }

		public Guid InstanceId { get; }

	    public override int SaveChanges()
		{
			var changes = base.SaveChanges();
			return changes;
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
            if (cancellationToken == default(CancellationToken))
                cancellationToken = CancellationToken.None;
			var changesAsync = await base.SaveChangesAsync(cancellationToken);
			return changesAsync;
		}

		public void SetState(object entity, EntityState state)
        {
            base.Entry(entity).State = state;
        }

        public void TrackGraphForAdd(object entity)
        {
            this.ChangeTracker.TrackGraph(entity, e => e.Entry.State = EntityState.Added);
        }
    }
}
