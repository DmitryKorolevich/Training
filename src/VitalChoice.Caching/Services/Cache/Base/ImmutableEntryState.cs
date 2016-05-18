using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

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