using System;
using System.Collections.Generic;
using Templates.Helpers;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowActionTree<TContext, TResult> : WorkflowExecutor<TContext, TResult>,
        IWorkflowActionTree<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        private readonly Dictionary<string, IWorkflowExecutor<TContext, TResult>> _actions;

        private readonly Dictionary<Type, string> _reverseAccessActions;

        protected WorkflowActionTree(IWorkflowActionTree<TContext, TResult> tree, string actionName) : base(null, actionName)
        {
            
        }

        protected WorkflowActionTree(HashSet<ActionItem> actionMapping, string actionName) : base(null, actionName)
        {
            _actions = new Dictionary<string, IWorkflowExecutor<TContext, TResult>>();
            _reverseAccessActions = new Dictionary<Type, string>();
            foreach (var action in actionMapping)
            {
                IWorkflowExecutor<TContext, TResult> instance;
                if (action.ActionType.IsImplement<IWorkflowActionTree<TContext, TResult>>())
                {
                    instance = (IWorkflowExecutor<TContext, TResult>)Activator.CreateInstance(action.ActionType, action.OptionalDependentTree, action.ActionName);
                }
                else
                {
                    instance = (IWorkflowExecutor<TContext, TResult>) Activator.CreateInstance(action.ActionType, this, action.ActionName);
                }
                _actions.Add(action.ActionName, instance);
                _reverseAccessActions.Add(action.ActionType, action.ActionName);
            }
        }

        public TResult GetActionResult(string actionName, TContext context)
        {
            TResult result;
            if (context.DictionaryData.TryGetValue(actionName, out result)) {
                return result;
            }
            return GetAction(actionName).Execute(context);
        }

        public bool TryGetActionResult(string actionName, TContext context, out TResult result)
        {
            return context.DictionaryData.TryGetValue(actionName, out result);
        }

        public TResult Execute(string actionName, TContext context)
        {
            var action = GetAction(actionName);
            return action.Execute(context);
        }

        public TResult Execute<TAction>(TContext context) 
            where TAction : IWorkflowExecutor<TContext, TResult>
        {
            var action = GetAction<TAction>();
            return action.Execute(context);
        }

        public virtual IWorkflowExecutor<TContext, TResult> GetAction<TAction>() 
            where TAction : IWorkflowExecutor<TContext, TResult>
        {
            return GetAction(_reverseAccessActions[typeof (TAction)]);
        }

        public virtual IWorkflowExecutor<TContext, TResult> GetAction(string actionName)
        {
            if (actionName == null)
                throw new ArgumentNullException(nameof(actionName));
            IWorkflowExecutor<TContext, TResult> result;
            if (!_actions.TryGetValue(actionName, out result))
            {
                throw new ApiException("ActionNotFound", actionName);
            }
            return result;
        }

        public virtual void SetUpActionDependencies(Dictionary<string, HashSet<string>> flatDependencyList)
        {
            if (flatDependencyList == null)
                throw new ArgumentNullException(nameof(flatDependencyList));
            foreach (var dependency in flatDependencyList)
            {
                var executor = GetAction(dependency.Key);
                var action = executor as IWorkflowAction<TContext, TResult>;
                if (action == null)
                    throw new ApiException("ActionDoesNotImplement", dependency.Key,
                        typeof (IWorkflowAction<TContext, TResult>));
                action.DependendActions.AddRange(dependency.Value);
            }
        }

        public void SetUpActionResolverDependencies(Dictionary<string, Dictionary<int, string>> flatDependencyList)
        {
            if (flatDependencyList == null)
                throw new ArgumentNullException(nameof(flatDependencyList));
            foreach (var dependency in flatDependencyList)
            {
                var executor = GetAction(dependency.Key);
                var action = executor as IWorkflowActionResolver<TContext, TResult>;
                if (action == null)
                    throw new ApiException("ActionDoesNotImplement", dependency.Key,
                        typeof (IWorkflowActionResolver<TContext, TResult>));
                foreach (var dependendAction in dependency.Value)
                {
                    action.Actions.Add(dependendAction.Key, dependendAction.Value);
                }
            }
        }

        /// <summary>
        /// Implement this to compute tree with default start action
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract override TResult Execute(TContext context);
    }
}