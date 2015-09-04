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

        public ActionResolverSetup()
        {
            Actions = new Dictionary<int, WorkflowActionResolverPathDefinition>();
        }

        public IActionResolverSetup<TContext, TResult> Action<T>(int key, string pathName)
            where T : IWorkflowAction<TContext, TResult>
        {
            Actions.Add(key, new WorkflowActionResolverPathDefinition
            {
                Type = typeof (T),
                Name = pathName,
                Path = key
            });
            return this;
        }
    }
}