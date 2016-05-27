using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheServiceScopeFactoryContainer
    {
        IServiceScopeFactory ScopeFactory { get; }
    }
}
