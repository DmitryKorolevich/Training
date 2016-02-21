using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;

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
    }
}