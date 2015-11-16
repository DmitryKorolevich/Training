using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;
using VitalChoice.Workflow.Data;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionResolverSetup<TContext, TResult> : IActionResolverSetup<TContext, TResult> 
        where TContext : WorkflowDataContext<TResult>
    {
        internal Dictionary<int, WorkflowActionResolverPathDefinition> Actions { get; }
        internal HashSet<Type> Dependencies { get; }

        public ActionResolverSetup()
        {
            Actions = new Dictionary<int, WorkflowActionResolverPathDefinition>();
            Dependencies = new HashSet<Type>();
        }

        public IActionResolverSetup<TContext, TResult> ResolvePath<T>(int key, string pathName)
            where T : IWorkflowExecutor<TContext, TResult>
        {
            Actions.Add(key, new WorkflowActionResolverPathDefinition
            {
                Type = typeof (T),
                Name = pathName,
                Path = key
            });
            return this;
        }

        public IActionResolverSetup<TContext, TResult> Dependency<T>() 
            where T : IWorkflowExecutor<TContext, TResult>
        {
            Dependencies.Add(typeof(T));
            return this;
        }
    }
}