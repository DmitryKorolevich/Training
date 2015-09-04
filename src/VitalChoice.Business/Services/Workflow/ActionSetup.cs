using System;
using System.Collections.Generic;
using Shared.Helpers;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionSetup<TContext, TResult> : IActionSetup<TContext, TResult> 
        where TContext : WorkflowContext<TResult>
    {
        public ActionSetup()
        {
            Actions = new HashSet<Type>();
        }

        internal HashSet<Type> Actions { get; }

        public IActionSetup<TContext, TResult> Action<T>() 
            where T : IWorkflowAction<TContext, TResult>
        {
            if (!typeof(T).IsImplementGeneric(typeof(IWorkflowAction<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowAction<TContext, TResult>");
            }
            Actions.Add(typeof(T));
            return this;
        }

        public IActionSetup<TContext, TResult> ActionResolver<T>() 
            where T : IWorkflowActionResolver<TContext, TResult>
        {
            if (!typeof(T).IsImplementGeneric(typeof(IWorkflowActionResolver<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowActionResolver<TContext, TResult>");
            }
            Actions.Add(typeof (T));
            return this;
        }
    }
}