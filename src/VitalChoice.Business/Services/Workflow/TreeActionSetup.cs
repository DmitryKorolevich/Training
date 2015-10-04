using System;
using System.Collections.Generic;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class TreeActionSetup<TContext, TResult> : ITreeActionSetup<TContext, TResult>
        where TContext : WorkflowContext<TResult>
    {
        public TreeActionSetup()
        {
            Actions = new HashSet<Type>();
        }

        internal HashSet<Type> Actions { get; }

        public ITreeActionSetup<TContext, TResult> Action<T>()
            where T : IWorkflowExecutor<TContext, TResult>
        {
            Actions.Add(typeof(T));
            return this;
        }
    }
}