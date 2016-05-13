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
        private static readonly object LockObject = new object();

        private static int _currentId;

        private static readonly string DataName = Guid.NewGuid().ToString();

        private static readonly ProfilingScope[] EmptyList = new ProfilingScope[0];

        private Stopwatch _stopwatch;
        private volatile List<ProfilingScope> _subScopes;
        private readonly Stack<ProfilingScope> _scopeStack;

        public ProfilingScope(object data, int skipFrames = 1)
        {
            if (Enabled)
            {
                lock (LockObject)
                {
                    _currentId++;
                    Id = _currentId;
                    _scopeStack = GetProfileStack();
                    _scopeStack.Push(this);
                }
                _stopwatch = new Stopwatch();
#if !DOTNET5_4
                var stackFrame = new StackFrame(skipFrames, false);
                var method = stackFrame.GetMethod();
                ClassType = method.DeclaringType;
                MethodName = method.Name;
                if (data == null)
                {
                    data = MethodName;
                }
#endif
                Data = data;
                _stopwatch.Start();
            }
        }

        public override string ToString()
        {
            if (Enabled)
            {
#if !DOTNET5_4
                return
                    $"{{\"{ClassType.FullName}::{MethodName}\":\"{Data?.ToString().Replace("\"", "\\\"")}\", \"time\": {TimeElapsed.TotalMilliseconds}{(AdditionalData == null ? string.Empty : $", \"additional\": [{string.Join(",", AdditionalData.Select(d => $"\"{d}\""))}]")} {(_subScopes == null ? string.Empty : $", \"subTrace\": [{string.Join(",", _subScopes.Select(s => s.ToString()))}]")}}}";
#else
            return Data?.ToString();
#endif
            }
            return string.Empty;
        }

        public int GetStackCount()
        {
            return _scopeStack?.Count ?? 0;
        }

#if !DOTNET5_4

        public static ProfilingScope GetRootScope()
        {
            var stack = GetProfileStack();
            lock (stack)
            {
                return stack.Count == 0 ? null : stack.Last();
            }
        }

        private static Stack<ProfilingScope> GetProfileStack()
        {
            var stack = (Stack<ProfilingScope>) CallContext.LogicalGetData(DataName);
            if (stack == null)
            {
                stack = new Stack<ProfilingScope>();
                CallContext.LogicalSetData(DataName, stack);
            }
            return stack;
        }
#else
        private static Stack<ProfilingScope> GetProfileStack()
        {
            return null;
        }
#endif
        public long Id { get; }

        public static bool Enabled { get; set; }

        public List<object> AdditionalData { get; private set; }

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
            if (Enabled)
            {
                lock (LockObject)
                {
                    if (AdditionalData == null)
                    {
                        AdditionalData = new List<object>();
                    }
                    AdditionalData.Add(data);
                }
            }
        }

        ~ProfilingScope()
        {
            Dispose(false);
        }

        private bool _disposed;

        protected void Dispose(bool disposing)
        {
            if (Enabled)
            {
                Stop();
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
                if (!_disposed && _scopeStack != null)
                {
                    lock (LockObject)
                    {
                        _disposed = true;
                        if (_scopeStack.Count > 0)
                        {
                            _scopeStack.Pop();
                            if (_scopeStack.Count > 0)
                            {
                                var parent = _scopeStack.Peek();
                                if (parent._subScopes == null)
                                {
                                    parent._subScopes = new List<ProfilingScope>();
                                }
                                lock (parent._subScopes)
                                {
                                    parent._subScopes.Add(this);
                                }
                            }
                        }
                    }
                }
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