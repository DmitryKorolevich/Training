using System.Collections.Generic;
using System.Dynamic;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowDataContext<T>
    {
        private readonly UnsafeDynamicObject _data;
        private readonly SafeDynamicObject _safeData;

        protected WorkflowDataContext()
        {
            _data = new UnsafeDynamicObject();
            _actionRunningList = new HashSet<string>();
            _safeData = new SafeDynamicObject(_data);
        }

        public dynamic Data => _data;

        public dynamic SafeData => _safeData;

        public IDictionary<string, object> DictionaryData => _data.Dictionary;

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