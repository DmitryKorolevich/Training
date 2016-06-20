using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Business.Services.Workflow
{
    public class TreeActionSetup<TContext, TResult> : ITreeActionSetup<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        public TreeActionSetup()
        {
            Actions = new Dictionary<Type, WorkflowActionDefinition>();
            ActionResolvers = new Dictionary<Type, WorkflowActionResolverDefinition>();
        }

        internal Dictionary<Type, WorkflowActionDefinition> Actions { get; }
        internal Dictionary<Type, WorkflowActionResolverDefinition> ActionResolvers { get; }

        public ITreeActionSetup<TContext, TResult> Action<T>(string actionName,
            Action<IActionSetup<TContext, TResult>> actions = null)
            where T : IWorkflowAction<TContext, TResult>
        {
            var action = new WorkflowActionDefinition(typeof(T), actionName);
            if (actions != null)
            {
                var actionSetup = new ActionSetup<TContext, TResult>();
                actions(actionSetup);
                action.Dependencies = actionSetup.Dependencies;
                action.Aggregations = actionSetup.Aggregations;
            }
            Actions.Add(typeof(T), action);
            return this;
        }

        public ITreeActionSetup<TContext, TResult> ActionResolver<T>(string actionName,
            Action<IActionResolverSetup<TContext, TResult>> actions)
            where T : IWorkflowActionResolver<TContext, TResult>
        {
            if (actions == null)
                throw new ArgumentNullException(nameof(actions));

            var action = new WorkflowActionResolverDefinition(typeof(T), actionName);
            var actionSetup = new ActionResolverSetup<TContext, TResult>();
            actions(actionSetup);
            action.Actions = actionSetup.Actions;
            action.Dependencies = actionSetup.Dependencies;
            ActionResolvers.Add(typeof(T), action);
            return this;
        }
    }
}