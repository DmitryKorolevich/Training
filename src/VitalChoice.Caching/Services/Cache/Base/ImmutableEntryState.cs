using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Infrastructure;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct ImmutableEntryState
    {
        private readonly InternalEntityEntry _entry;

        public EntityState State { get; }

        public object Entity => _entry.Entity;

        public Type EntityType => _entry.EntityType?.ClrType;

        public ImmutableEntryState(InternalEntityEntry entry)
        {
            _entry = entry;
            State = entry.EntityState;
        }

        public ImmutableEntryState(EntityEntry entry)
        {
            _entry = ((IInfrastructure<InternalEntityEntry>) entry).Instance;
            State = entry.State;
        }
    }
}