using System;
using System.Collections.Generic;
using Shared.Helpers;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Workflow
{
    public class ActionResolverSetup : IActionResolverSetup
    {
        internal Dictionary<int, Type> Actions { get; }

        public ActionResolverSetup()
        {
            Actions = new Dictionary<int, Type>();
        }

        public IActionResolverSetup Action<T>(int key)
        {
            if (!typeof (T).IsImplementGeneric(typeof (IWorkflowAction<,>)))
            {
                throw new ApiException($"Type {typeof(T)} doesn't implement IWorkflowAction<TContext, TResult>");
            }
            Actions.Add(key, typeof(T));
            return this;
        }
    }
}