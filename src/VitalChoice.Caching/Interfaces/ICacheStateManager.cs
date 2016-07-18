using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheStateManager : IStateManager
    {
        object GetTrackedOrTrackEntity(EntityInfo info, object entity);
        IEnumerable<object> GetTrackedOrTrackEntity(EntityInfo info, IEnumerable<object> entities);
    }
}