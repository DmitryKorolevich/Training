using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionSetup<TContext, TResult> : IActionSetup<TContext, TResult> 
        where TContext : WorkflowDataContext<TResult>
    {
        public ActionSetup()
        {
            Aggregations = new HashSet<Type>();
            Dependencies = new HashSet<Type>();
        }

        internal HashSet<Type> Aggregations { get; }
        internal HashSet<Type> Dependencies { get; }

        public IActionSetup<TContext, TResult> Aggregate<T>() 
            where T : IWorkflowExecutor<TContext, TResult>
        {
            Aggregations.Add(typeof(T));
            return this;
        }

        public IActionSetup<TContext, TResult> Dependency<T>() where T : IWorkflowExecutor<TContext, TResult>
        {
            Dependencies.Add(typeof(T));
            return this;
        }
    }
}