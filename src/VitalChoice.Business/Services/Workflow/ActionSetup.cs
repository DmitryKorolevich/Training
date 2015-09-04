using System;
using System.Collections.Generic;
using Shared.Helpers;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionSetup : IActionSetup
    {
        public ActionSetup()
        {
            Actions = new HashSet<Type>();
        }

        internal HashSet<Type> Actions { get; }

        public IActionSetup Action<T>()
        {
            if (!typeof(T).IsImplementGeneric(typeof(IWorkflowAction<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowAction<TContext, TResult>");
            }
            Actions.Add(typeof(T));
            return this;
        }

        public IActionSetup ActionResolver<T>()
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