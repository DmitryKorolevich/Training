﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Workflow.Core
{
    public interface IWorkflowTree<TContext, TResult>
        where TContext : WorkflowDataContext<TResult>
    {
        Task<TResult> Execute(TContext context);

        Task<TResult> Execute(string actionName, TContext result);

        Task<TResult> Execute<TAction>(TContext context) where TAction : IWorkflowExecutor<TContext, TResult>;

        IWorkflowExecutor<TContext, TResult> GetAction(string actionName);

        IWorkflowExecutor<TContext, TResult> GetAction<TAction>() where TAction : IWorkflowExecutor<TContext, TResult>;

        Task<TResult> GetActionResult(string actionName, TContext context);

        bool TryGetActionResult(string actionName, TContext context, out TResult result);

        Task InitializeTreeAsync();
    }
}