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
            _actionRunningList = new HashSet<string>();
        }

        public dynamic Data => _data;

        public IDictionary<string, object> DictionaryData => _data as IDictionary<string, object>;

        private readonly HashSet<string> _actionRunningList;

        internal void ActionLock(string name)
        {
            if (_actionRunningList.Contains(name))
                throw new ApiException("ActionRecursionDetected", name);
            _actionRunningList.Add(name);
        }

        internal void ActionUnlock(string name)
        {
            if (!_actionRunningList.Contains(name))
                throw new ApiException("ActionExitedTwice", name);
            _actionRunningList.Remove(name);
        }

        internal void ActionSetResult(string actionName, T data)
        {
            if (DictionaryData.ContainsKey(actionName))
            {
                throw new ApiException("ActionAlreadySetData", actionName);
            }
            DictionaryData.Add(actionName, data);
        }
    }
}