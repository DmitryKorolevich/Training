using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Business.Services
{
    public class PerformanceRequestService : DefaultPerformanceRequest
    {
        private const int MaxScopes = 1000;

        private static readonly object LockObject = new object();

        private static readonly ScopeComparer Comparer = new ScopeComparer();

        private static readonly ProfilingScope[] Scopes = new ProfilingScope[MaxScopes];

        public PerformanceRequestService(ILoggerFactory factory) : base(factory)
        {

        }

        public static IReadOnlyCollection<ProfilingScope> GetWorkedScopes()
        {
            lock (LockObject)
            {
                return Scopes.Where(s => s != null).ToArray();
            }
        }

        public static void Clear()
        {
            lock (LockObject)
            {
                for (int i = 0; i < Scopes.Length; i++)
                {
                    Scopes[i] = null;
                }
            }
        }

        public override void OnFinishedRequest(ProfilingScope scope)
        {
            base.OnFinishedRequest(scope);

            lock (LockObject)
            {
                var index = Array.BinarySearch(Scopes, scope, Comparer);
                if (index < 0)
                {
                    index = ~index;
                    if (index >= 0 && index < MaxScopes)
                    {
                        if (Scopes[index] != null)
                        {
                            for (var i = MaxScopes - 1; i > index; i--)
                            {
                                Scopes[i] = Scopes[i - 1];
                            }
                        }
                        Scopes[index] = scope;
                    }
                }
                else
                {
                    if (index >= 0 && index < MaxScopes)
                    {
                        for (var i = MaxScopes - 1; i > index; i--)
                        {
                            Scopes[i] = Scopes[i - 1];
                        }
                        Scopes[index] = scope;
                    }
                }
            }
        }

        private struct ScopeComparer : IComparer<ProfilingScope>
        {
            public int Compare(ProfilingScope x, ProfilingScope y)
            {
                if (x == y)
                {
                    return 0;
                }
                if (x == null)
                {
                    return 1;
                }
                if (y == null)
                {
                    return -1;
                }
                return y.TimeElapsed.CompareTo(x.TimeElapsed);
            }
        }
    }
}