using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Templates.Helpers;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Workflow;

namespace VitalChoice.Workflow.Core
{
    public abstract class WorkflowTree<TContext, TResult> : WorkflowExecutor<TContext, TResult>,
        IWorkflowTree<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        private readonly IActionItemProvider _itemProvider;
        private readonly Dictionary<string, IWorkflowExecutor<TContext, TResult>> _actions;

        private readonly Dictionary<Type, string> _reverseAccessActions;

        protected WorkflowTree(IActionItemProvider itemProvider, string treeName) : base(null, treeName)
        {
            _itemProvider = itemProvider;
            _actions = new Dictionary<string, IWorkflowExecutor<TContext, TResult>>();
            _reverseAccessActions = new Dictionary<Type, string>();
        }

        public TResult GetActionResult(string actionName, TContext context)
        {
            object result;
            if (context.DictionaryData.TryGetValue(actionName, out result)) {
                return (TResult)result;
            }
            return GetAction(actionName).Execute(context);
        }

        public bool TryGetActionResult(string actionName, TContext context, out TResult result)
        {
            object value;
            var getResult = context.DictionaryData.TryGetValue(actionName, out value);
            result = (TResult)value;
            return getResult;
        }

        public async Task InitializeTreeAsync()
        {
            var actionMapping = await _itemProvider.GetDependencyItems(Name);
            foreach (var action in actionMapping)
            {
                await AddWalkAction(action);
            }
        }

        private async Task AddTreeWithDependencies(ActionItem action)
        {
            var workflowTree =
                (IWorkflowTree<TContext, TResult>) Activator.CreateInstance(action.ActionType, _itemProvider, action.ActionName);
            _actions.Add(action.ActionName, workflowTree);
            _reverseAccessActions.Add(action.ActionType, action.ActionName);
            await workflowTree.InitializeTreeAsync();
        }

        private async Task AddActionWithDependencies(ActionItem action)
        {
            var workflowAction =
                (IWorkflowAction<TContext, TResult>)
                    Activator.CreateInstance(action.ActionType, this, action.ActionName);
            _actions.Add(action.ActionName, workflowAction);
            _reverseAccessActions.Add(action.ActionType, action.ActionName);
            var dependencyItems = await _itemProvider.GetActionDependencyItems(action.ActionName);
            foreach (var dep in dependencyItems)
            {
                workflowAction.DependendActions.Add(dep.ActionName);
                if (!_actions.ContainsKey(dep.ActionName))
                {
                    await AddWalkAction(dep);
                }
            }
        }

        private async Task AddWalkAction(ActionItem action)
        {
            switch (action.WorkflowActionType)
            {
                case WorkflowActionType.ActionTree:
                    await AddTreeWithDependencies(action);
                    break;
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
            var dependencyItems = await _itemProvider.GetActionResolverDependencyItems(action.ActionName);
            foreach (var dep in dependencyItems)
            {
                workflowActionResolver.Actions.Add(dep.Key, dep.Value.ActionName);
                if (!_actions.ContainsKey(dep.Value.ActionName))
                {
                    await AddWalkAction(dep.Value);
                }
            }
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

        /// <summary>
        /// Implement this to compute tree with default start action
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract override TResult Execute(TContext context);
    }
}