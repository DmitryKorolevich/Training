using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowContext<T>
    {
        private readonly ExpandoObject _data;

        protected WorkflowContext()
        {
            _data = new ExpandoObject();
            ActionRunningList = new HashSet<string>();
            DictionaryData = new Dictionary<string, T>();
        }

        public dynamic Data => _data;

        public Dictionary<string, T> DictionaryData { get; }

        internal HashSet<string> ActionRunningList { get; }

        internal void ActionLock(string name)
        {
            if (ActionRunningList.Contains(name))
                throw new ApiException("ActionRecursionDetected", name);
            ActionRunningList.Add(name);
        }

        internal void ActionUnlock(string name)
        {
            if (!ActionRunningList.Contains(name))
                throw new ApiException("ActionExitedTwice", name);
            ActionRunningList.Remove(name);
        }

        internal void ActionSetResult(string actionName, T data)
        {
            var resultData = (_data as IDictionary<string, object>);
            if (resultData.ContainsKey(actionName))
            {
                throw new ApiException("ActionAlreadySetData", actionName);
            }
            DictionaryData.Add(actionName, data);
            resultData.Add(actionName, data);
        }
    }
}