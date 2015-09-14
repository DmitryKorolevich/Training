using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionSetup<TContext, TResult> : IActionSetup<TContext, TResult> 
        where TContext : WorkflowContext<TResult>
    {
        public ActionSetup()
        {
            Dependencies = new HashSet<Type>();
        }

        internal HashSet<Type> Dependencies { get; }

        public IActionSetup<TContext, TResult> Dependency<T>() 
            where T : IWorkflowExecutor<TContext, TResult>
        {
            Dependencies.Add(typeof(T));
            return this;
        }
    }
}