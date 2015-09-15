using System;
using System.Collections.Generic;
using Shared.Helpers;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Workflow;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionResolverSetup<TContext, TResult> : IActionResolverSetup<TContext, TResult> 
        where TContext : WorkflowContext<TResult>
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