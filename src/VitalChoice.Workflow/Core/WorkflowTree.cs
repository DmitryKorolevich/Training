using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Workflow;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Profiling.Base;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowTree<TContext, TResult>:
        IWorkflowTree<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        private readonly IActionItemProvider _itemProvider;
        private readonly Dictionary<string, IWorkflowExecutor<TContext, TResult>> _actions;

        private readonly Dictionary<Type, string> _reverseAccessActions;

        protected WorkflowTree(IActionItemProvider itemProvider, string treeName)
        {
            Name = treeName;
            _itemProvider = itemProvider;
            _actions = new Dictionary<string, IWorkflowExecutor<TContext, TResult>>();
            _reverseAccessActions = new Dictionary<Type, string>();
        }

        public string Name { get; }

        public async Task<TResult> GetActionResultAsync(string actionName, TContext context)
        {
            object result;
            if (context.DictionaryData.TryGetValue(actionName, out result)) {
                return (TResult)result;
            }
            using (new ProfilingScope($"Workflow Tree: {Name}"))
            {
                using (var executionContext = new AutofacExecutionContext())
                {
                    return await GetAction(actionName).ExecuteAsync(context, executionContext);
                }
            }
        }

        public bool TryGetActionResult(string actionName, TContext context, out TResult result)
        {
            object value;
            var getResult = context.DictionaryData.TryGetValue(actionName, out value);
            if (value != null)
            {
                result = (TResult) value;
            }
            else
            {
                result = default(TResult);
            }
            return getResult;
        }

        public async Task InitializeTreeAsync()
        {
            var actionMapping = await _itemProvider.GetTreeActions(Name);
            foreach (var action in actionMapping)
            {
                await AddWalkAction(action);
            }
        }

        private async Task AddActionWithDependencies(ActionItem action)
        {
            var workflowAction =
                (IWorkflowAction<TContext, TResult>)
                    Activator.CreateInstance(action.ActionType, this, action.ActionName);
            _actions.Add(action.ActionName, workflowAction);
            _reverseAccessActions.Add(action.ActionType, action.ActionName);
            var dependencyItems = await _itemProvider.GetDependencies(action.ActionName, action.ActionType);
            foreach (var dep in dependencyItems)
            {
                workflowAction.DependendActions.Add(dep.ActionName);
                if (!_actions.ContainsKey(dep.ActionName))
                {
                    await AddWalkAction(dep);
                }
            }
            var aggregatedItems = await _itemProvider.GetAggregations(action.ActionName, action.ActionType);
            foreach (var aggr in aggregatedItems)
            {
                workflowAction.AggreagatedActions.Add(aggr.ActionName);
                if (!_actions.ContainsKey(aggr.ActionName))
                {
                    await AddWalkAction(aggr);
                }
            }
        }

        private async Task AddWalkAction(ActionItem action)
        {
            switch (action.WorkflowActionType)
            {
                case WorkflowActionType.Action:
                    await AddActionWithDependencies(action);
                    break;
                case WorkflowActionType.ActionResolver:
                    await AddActionResolverWithDependencies(action);
                    break;
                default:
                    throw new ApiException("Invalid action type supplied");
            }
        }

        private async Task AddActionResolverWithDependencies(ActionItem action)
        {
            var workflowActionResolver =
                (IWorkflowActionResolver<TContext, TResult>)
                    Activator.CreateInstance(action.ActionType, this, action.ActionName);
            _actions.Add(action.ActionName, workflowActionResolver);
            _reverseAccessActions.Add(action.ActionType, action.ActionName);
            var dependencyItems = await _itemProvider.GetDependencies(action.ActionName, action.ActionType);
            foreach (var dep in dependencyItems)
            {
                workflowActionResolver.DependendActions.Add(dep.ActionName);
                if (!_actions.ContainsKey(dep.ActionName))
                {
                    await AddWalkAction(dep);
                }
            }
            var paths = await _itemProvider.GetActionResolverPaths(action.ActionName, action.ActionType);
            foreach (var path in paths)
            {
                workflowActionResolver.Actions.Add(path.Key, path.Value.ActionName);
                if (!_actions.ContainsKey(path.Value.ActionName))
                {
                    await AddWalkAction(path.Value);
                }
            }
        }

        public async Task<TResult> ExecuteAsync(string actionName, TContext context)
        {
            var action = GetAction(actionName);
            using (new ProfilingScope($"Workflow Tree: {Name}"))
            {
                using (var executionContext = new AutofacExecutionContext())
                {
                    return await action.ExecuteAsync(context, executionContext);
                }
            }
        }

        public async Task<TResult> ExecuteAsync<TAction>(TContext context) 
            where TAction : IWorkflowExecutor<TContext, TResult>
        {
            var action = GetAction<TAction>();
            using (new ProfilingScope($"Workflow Tree: {Name}"))
            {
                using (var executionContext = new AutofacExecutionContext())
                {
                    return await action.ExecuteAsync(context, executionContext);
                }
            }
        }

        public IWorkflowExecutor<TContext, TResult> GetAction<TAction>() 
            where TAction : IWorkflowExecutor<TContext, TResult>
        {
            return GetAction(_reverseAccessActions[typeof (TAction)]);
        }

        public IWorkflowExecutor<TContext, TResult> GetAction(string actionName)
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

        /// <summary>
        /// Implement this to compute tree with default start action
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<TResult> ExecuteAsync(TContext context);
    }
}