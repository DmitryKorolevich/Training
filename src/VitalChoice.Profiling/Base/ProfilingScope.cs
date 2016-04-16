using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
#if !DOTNET5_4
using System.Runtime.Remoting.Messaging;
#endif
using System.Threading;

namespace VitalChoice.Profiling.Base
{
    public class ProfilingScope : IDisposable
    {
        private static readonly string DataName = Guid.NewGuid().ToString();

        private static readonly ProfilingScope[] EmptyList = new ProfilingScope[0];

#if !DOTNET5_4
        private class Wrapper : MarshalByRefObject
        {
            public Stack<ProfilingScope> Value { get; set; }
        }

        public static ProfilingScope RootScope => ProfileScope.Count == 0 ? null : ProfileScope.Last();

        private static Stack<ProfilingScope> ProfileScope
        {
            get
            {
                var stack = (Wrapper) CallContext.LogicalGetData(DataName);
                if (stack == null)
                {
                    stack = new Wrapper
                    {
                        Value = new Stack<ProfilingScope>()
                    };
                    CallContext.LogicalSetData(DataName, stack);
                }
                return stack.Value;
            }
        }
#else
        private static Stack<ProfilingScope> ProfileScope
        {
            get
            {
                return new Stack<ProfilingScope>();
            }
        }
#endif

        private Stopwatch _stopwatch;
        private ConcurrentBag<ProfilingScope> _subScopes;
        public List<object> AdditionalData { get; private set; }

        public ProfilingScope(object data)
        {
            _stopwatch = new Stopwatch();
#if !DOTNET5_4
            var stackFrame = new StackFrame(1, false);
            var method = stackFrame.GetMethod();
            ClassType = method.DeclaringType;
            MethodName = method.Name;
#endif
            Data = data;
            if (Current != null)
            {
                if (Current._subScopes == null)
                {
                    Current._subScopes = new ConcurrentBag<ProfilingScope>();
                }
                Current._subScopes.Add(this);
            }
            ProfileScope.Push(this);
            _stopwatch.Start();
        }

        public override string ToString()
        {
#if !DOTNET5_4
            return
                $"{{\"{ClassType.FullName}::{MethodName}\":\"{Data?.ToString().Replace("\"", "\\\"")}\", \"time\": {TimeElapsed.TotalMilliseconds}{(AdditionalData == null ? string.Empty : $", \"additional\": [{string.Join(",", AdditionalData.Select(d => $"\"{d}\""))}]")} {(_subScopes == null ? string.Empty : $", \"subTrace\": [{string.Join(",", _subScopes.Select(s => s.ToString()))}]")}}}";
#else
            return Data?.ToString();
#endif
        }

        public static ProfilingScope Current => ProfileScope.Count == 0 ? null : ProfileScope.Peek();

        public static void InitializeRoot(string requestUrl)
        {
            ProfileScope.Push(new ProfilingScope(requestUrl));
        }

        public static ProfilingScope DeallocateRoot()
        {
            return ProfileScope.Pop();
        }

        public IReadOnlyCollection<ProfilingScope> SubScopes => _subScopes?.ToArray() ?? EmptyList;

        public Type ClassType { get; }

        public string MethodName { get; }

        public TimeSpan TimeElapsed { get; private set; }

        public object Data { get; }
        public bool ForceLog { get; set; }

        public void AddScopeData(object data)
        {
            if (data == null)
                return;
            if (AdditionalData == null)
            {
                AdditionalData = new List<object>();
            }
            lock (AdditionalData)
            {
                AdditionalData.Add(data);
            }
        }

        ~ProfilingScope()
        {
            Dispose(false);
        }

        private bool _disposed;

        protected void Dispose(bool disposing)
        {
            Stop();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            if (!_disposed)
            {
                _disposed = true;
                ProfileScope.Pop();
            }
        }

        public void Stop()
        {
            if (_stopwatch != null)
            {
                _stopwatch.Stop();
                TimeElapsed = _stopwatch.Elapsed;
                _stopwatch = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}