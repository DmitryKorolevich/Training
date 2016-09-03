using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheStateManager : IStateManager
    {
        bool IsTracked(EntityInfo info, EntityKey pk, object entity, bool keyOnly = false);
        void AcceptTrackData();
        void RejectTrackData();
        object GetOrAddTracked(EntityInfo info, object entity, out bool hasCloned);
        IEnumerable<object> GetOrAddTracked(EntityInfo info, IEnumerable<object> entities);
    }
}